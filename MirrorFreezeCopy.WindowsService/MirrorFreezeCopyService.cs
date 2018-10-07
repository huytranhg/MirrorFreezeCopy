// <copyright file="MirrorFreezeCopyService.cs" company="Huy Tran">
// Copyright (c) Huy Tran. All rights reserved.
// </copyright>

namespace MirrorFreezeCopy
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using MirrorFreezeCopy.Domain;
    using MirrorFreezeCopy.Domain.Interfaces;
    using MirrorFreezeCopy.Persistence;

    /// <summary>
    /// This is MirrorFreezeCopyService class that help Mirror And Freeze Windows Service to run.
    /// </summary>
    public class MirrorFreezeCopyService : IDisposable
    {
        private static readonly NLog.Logger NLogger = NLog.LogManager.GetCurrentClassLogger();
        private static readonly string ConfigFileName = "MirrorFreezeCopy_Config.xml";
        private static readonly string ConfigFilePath =
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                "MirrorFreezeCopy",
                ConfigFileName);

        private readonly WatcherConfigService watcherConfigService;
        private readonly IWatcherConfig watcherConfig;
        private readonly List<Watcher> listWatcher;
        private readonly List<WatcherExecute> listWatcherExecute;
        private readonly List<WindowsFileSystemWatcher> listWindowsFileSystemWatcher;
        private readonly RetryOption retryOption;
        private WatcherExecute runningWatcherExecute;
        private WindowsFileSystemWatcher runningWindowsFileSystemWatcher;

        /// <summary>
        /// Initializes a new instance of the <see cref="MirrorFreezeCopyService"/> class.
        /// </summary>
        public MirrorFreezeCopyService()
        {
            this.watcherConfig = new WatcherConfig();
            this.watcherConfigService = new WatcherConfigService(this.watcherConfig);
            this.Precheck();
            this.retryOption = this.watcherConfigService.GetRetryOptionFromXMLFile(ConfigFilePath);
            this.listWatcher = this.watcherConfigService.PopulateWatchersFromXMLFile(ConfigFilePath);
            this.listWatcherExecute = new List<WatcherExecute>();
            this.listWindowsFileSystemWatcher = new List<WindowsFileSystemWatcher>();
        }

        /// <summary>
        /// Start MirrorFreezeCopyService.
        /// </summary>
        public virtual void Start()
        {
            if (this.PostCheck())
            {
                foreach (Watcher runningWatcher in this.listWatcher)
                {
                    if (this.CheckWatcherSourceFolderExist(runningWatcher))
                    {
                        this.runningWatcherExecute = new WatcherExecute();
                        this.runningWindowsFileSystemWatcher = new WindowsFileSystemWatcher(this.runningWatcherExecute, this.retryOption);
                        this.runningWindowsFileSystemWatcher.Start(runningWatcher);
                        this.listWatcherExecute.Add(this.runningWatcherExecute);
                        this.listWindowsFileSystemWatcher.Add(this.runningWindowsFileSystemWatcher);
                    }
                }
            }
            else
            {
                NLogger.Info("There's no config to be run.");
            }
        }

        /// <summary>
        /// Dispose resources.
        /// </summary>
        public virtual void Dispose()
        {
            if (this.listWatcherExecute.Count > 0)
            {
                foreach (WatcherExecute runningWatcherExecute in this.listWatcherExecute)
                {
                    runningWatcherExecute.Dispose();
                }

                this.listWatcherExecute.Clear();
            }

            if (this.listWindowsFileSystemWatcher.Count > 0)
            {
                foreach (WindowsFileSystemWatcher runningWindowsFileSystemWatcher in this.listWindowsFileSystemWatcher)
                {
                    runningWindowsFileSystemWatcher.Dispose();
                }

                this.listWindowsFileSystemWatcher.Clear();
            }
        }

        private void Precheck()
        {
            if (!File.Exists(ConfigFilePath))
            {
                this.watcherConfigService.WriteSampleConfig(ConfigFilePath);
            }
        }

        private bool PostCheck()
        {
            if (this.listWatcher != null && this.listWatcher.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool CheckWatcherSourceFolderExist(Watcher watcher)
        {
            if (Directory.Exists(watcher.Source))
            {
                return true;
            }
            else
            {
                if (!Directory.Exists(watcher.Source))
                {
                    NLogger.Info(
                        "Source folder: "
                        + System.Environment.NewLine
                        + "{0}"
                        + System.Environment.NewLine
                        + "doesn't exist.", watcher.Source);
                }

                return false;
            }
        }
    }
}