// <copyright file="WindowsFileSystemWatcher.cs" company="Huy Tran">
// Copyright (c) Huy Tran. All rights reserved.
// </copyright>

namespace MirrorFreezeCopy.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using System.Threading;
    using MirrorFreezeCopy.Domain;
    using MirrorFreezeCopy.Domain.Enums;
    using MirrorFreezeCopy.Domain.Interfaces;

    /// <summary>
    /// Implementation of IWindowsFileSystemWatcher and IDisposable.
    /// </summary>
    public class WindowsFileSystemWatcher : IWindowsFileSystemWatcher, IDisposable
    {
        private static readonly NLog.Logger NLogger = NLog.LogManager.GetCurrentClassLogger();
        private readonly IWatcherExecute watcherExecute;
        private readonly RetryOption retryOption;
        private Watcher watcher;
        private FileSystemWatcher fileSystemWatcher;

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowsFileSystemWatcher"/> class.
        /// </summary>
        /// <param name="watcherExecute"> Instance of IWatcherExecute</param>
        /// <param name="retryOption"> Instance of RetryOption, default can be null</param>
        public WindowsFileSystemWatcher(IWatcherExecute watcherExecute, RetryOption retryOption = null)
        {
            // Domain or WindowsService needs to inject IWatcherExecute implementation to this WindowsFileSystemWatcher
            // to avoid coupling of WindowsFileSystemWatcher and WatcherExecute in Persistence Layer
            this.watcherExecute = watcherExecute;
            this.retryOption = retryOption;
        }

        /// <summary>
        /// Start WindowsFileSystemWatcher.
        /// </summary>
        /// <param name="watcher"> Instance of Watcher that has neccessary information for WindowsFileSystemWatcher to run.</param>
        public void Start(Watcher watcher)
        {
            this.watcher = watcher;

            if (!this.Precheck())
            {
                return;
            }

            if (Directory.Exists(this.watcher.Source))
            {
                this.StartWatcher();
            }
            else
            {
                NLogger.Info(
                    "Source folder:"
                    + Environment.NewLine
                    + "{0}"
                    + Environment.NewLine
                    + "does not exist."
                    + Environment.NewLine
                    + "Watcher will not start.",
                    this.watcher.Source);
            }
        }

        /// <summary>
        /// Dispose resources.
        /// </summary>
        public void Dispose()
        {
            if (this.fileSystemWatcher != null)
            {
                this.fileSystemWatcher.Dispose();
            }
        }

        private void StartWatcher()
        {
            try
            {
                this.fileSystemWatcher = new FileSystemWatcher();
                this.fileSystemWatcher.IncludeSubdirectories = true;
                this.fileSystemWatcher.Path = this.watcher.Source;
                this.fileSystemWatcher.NotifyFilter = NotifyFilters.LastWrite;
                this.fileSystemWatcher.Changed += new FileSystemEventHandler(this.OnChanged);

                this.fileSystemWatcher.EnableRaisingEvents = true;
            }
            catch (Exception ex)
            {
                NLogger.Error(ex, "Error while starting FileSystemWatcher.");
                this.Dispose();
            }
        }

        private void OnChanged(object source, FileSystemEventArgs e)
        {
            // For Freeze action, needs to disable EnableRaisingEvents first for Source folder.
            // After executing ROBOCOPY command line, then enable back.
            if (this.watcher.Action == WatcherAction.Freeze.ToString("g"))
            {
                this.fileSystemWatcher.EnableRaisingEvents = false;
                if (!this.PostcheckExistRetryOption())
                {
                    this.watcherExecute.Execute(this.watcher);
                }
                else
                {
                    this.watcherExecute.Execute(this.watcher, this.retryOption.NumberOfRetries, this.retryOption.Interval);
                }

                this.fileSystemWatcher.EnableRaisingEvents = true;
            }
            else
            {
                if (!this.PostcheckExistRetryOption())
                {
                    this.watcherExecute.Execute(this.watcher);
                }
                else
                {
                    this.watcherExecute.Execute(this.watcher, this.retryOption.NumberOfRetries, this.retryOption.Interval);
                }
            }
        }

        private bool Precheck()
        {
            if (this.watcher == null || this.watcherExecute == null)
            {
                NLogger.Info("Watcher and/or WatcherExecute is null.");
                return false;
            }
            else
            {
                return true;
            }
        }

        private bool PostcheckExistRetryOption()
        {
            if (this.retryOption != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}