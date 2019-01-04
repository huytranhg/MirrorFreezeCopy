// <copyright file="WatcherConfigService.cs" company="Huy Tran">
// Copyright (c) Huy Tran. All rights reserved.
// </copyright>

namespace MirrorFreezeCopy.Domain
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using AutoMapper;
    using MirrorFreezeCopy.Domain.DTO;
    using MirrorFreezeCopy.Domain.Enums;
    using MirrorFreezeCopy.Domain.Interfaces;

    /// <summary>
    /// Object to get objects, and logging from xml file.
    /// </summary>
    public class WatcherConfigService
    {
        private static readonly NLog.Logger NLogger = NLog.LogManager.GetCurrentClassLogger();
        private readonly IWatcherConfig watcherConfig;

        /// <summary>
        /// Initializes a new instance of the <see cref="WatcherConfigService"/> class.
        /// </summary>
        /// <param name="watcherConfig"> IWatcherConfig</param>
        public WatcherConfigService(IWatcherConfig watcherConfig)
        {
            this.watcherConfig = watcherConfig;
        }

        /// <summary>
        /// Populate list of Watcher objects from xml file.
        /// </summary>
        /// <param name="filePath"> Absolute path of xml file.</param>
        /// <returns>List of Watcher objects.</returns>
        public virtual List<Watcher> PopulateWatchersFromXMLFile(string filePath)
        {
            if (!this.PrecheckInterfaceExisted())
            {
                return null;
            }

            try
            {
                if (File.Exists(filePath))
                {
                    List<Watcher> listUncheckedWatcher = Mapper.Map<List<WatcherConfigDto>, List<Watcher>>(this.watcherConfig.ReadWatchers(filePath));
                    List<Watcher> listCheckedWatcher = new List<Watcher>();

                    if (this.PostcheckWatchersExist(listUncheckedWatcher))
                    {
                        foreach (Watcher runningWatcher in listUncheckedWatcher)
                        {
                            if (this.PostcheckSourceFolderExisted(runningWatcher)
                                && this.PostcheckDestinationFolderCanBeCreated(runningWatcher)
                                && !this.PostcheckWatcherIsRootFolder(runningWatcher)
                                && this.PostcheckIsDefinedAction(runningWatcher))
                            {
                                listCheckedWatcher.Add(runningWatcher);
                            }
                        }
                    }

                    if (!this.PostcheckWatchersExist(listCheckedWatcher))
                    {
                        listCheckedWatcher = null;
                        NLogger.Info("There's no valid config Watcher.");
                    }
                    else
                    {
                        NLogger.Info("Loaded valid Watchers: ");
                        foreach (Watcher runningWatcher in listCheckedWatcher)
                        {
                            NLogger.Info(
                                Environment.NewLine +
                                    "Action: {0}." +
                                    Environment.NewLine +
                                    "Source: {1}." +
                                    Environment.NewLine +
                                    "Destination: {2}.",
                                    runningWatcher.Action,
                                    runningWatcher.Source,
                                    runningWatcher.Destination);
                        }
                    }

                    return listCheckedWatcher;
                }
                else
                {
                    NLogger.Info("Config file does not exist.");
                    return null;
                }
            }
            catch (InvalidOperationException ex)
            {
                NLogger.Error(ex, "Error while reading config file.");
                return null;
            }
        }

        /// <summary>
        /// Get RetryOption object from xml file.
        /// </summary>
        /// <param name="filePath"> Absolute path of xml file.</param>
        /// <returns>RetryOption object.</returns>
        public virtual RetryOption GetRetryOptionFromXMLFile(string filePath)
        {
            if (!this.PrecheckInterfaceExisted())
            {
                return null;
            }

            try
            {
                if (File.Exists(filePath) && this.watcherConfig != null)
                {
                    RetryOption retryOption = Mapper.Map<RetryOptionConfigDto, RetryOption>(this.watcherConfig.ReadRetryOption(filePath));

                    if (this.PostcheckValidRetryOption(retryOption))
                    {
                        NLogger.Info(
                            "Loaded RetryOption: " +
                            Environment.NewLine +
                            "NumberOfRetries: {0}." +
                            Environment.NewLine +
                            "Interval: {1}.",
                            retryOption.NumberOfRetries,
                            retryOption.Interval);
                    }
                    else
                    {
                        NLogger.Info("There's no valid config for RetryOption. ROBOCOPY's default will be used.");
                    }

                    return retryOption;
                }
                else
                {
                    NLogger.Info("Config file does not exist.");
                    return null;
                }
            }
            catch (InvalidOperationException ex)
            {
                NLogger.Error(ex, "Error while reading config file.");
                return null;
            }
        }

        /// <summary>
        /// Write sample config to xml file.
        /// </summary>
        /// <param name="filePath"> Absolute path of xml file.</param>
        public virtual void WriteSampleConfig(string filePath)
        {
            if (!this.PrecheckInterfaceExisted())
            {
                return;
            }
            else
            {
                this.watcherConfig.WriteSampleConfig(filePath);
            }
        }

        private bool PrecheckInterfaceExisted()
        {
            if (this.watcherConfig == null)
            {
                NLogger.Info("WatcherConfig is null.");
                return false;
            }
            else
            {
                return true;
            }
        }

        private bool PostcheckValidRetryOption(RetryOption retryOption)
        {
            if (retryOption != null
                && retryOption.NumberOfRetries > 0
                && retryOption.Interval > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool PostcheckWatcherIsRootFolder(Watcher watcher)
        {
            // Do not start WindowsFileSystemWatcher if the folder to be copied from is root.
            // More coding will be required for ROBOCOPY to mirror correctly from root.
            // Therefor MirrorFreezeCopy will skipp the config for root folder.
            if (watcher.Action == WatcherAction.Freeze.ToString("g"))
            {
                DirectoryInfo directoryInfoDestination = new DirectoryInfo(watcher.Destination);
                if (directoryInfoDestination.Parent == null)
                {
                    NLogger.Info(
                        "Invalid Watcher:"
                        + Environment.NewLine
                        + "Action: {0}."
                        + Environment.NewLine
                        + "Source: {1}."
                        + Environment.NewLine
                        + "Destination: {2}."
                        + Environment.NewLine
                        + "Watcher Destination folder:"
                        + Environment.NewLine
                        + "{2}"
                        + Environment.NewLine
                        + "is root folder."
                        + Environment.NewLine
                        + "MirrorFreezeCopy will not Freeze from this folder.",
                        watcher.Action,
                        watcher.Source,
                        watcher.Destination);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                DirectoryInfo directoryInfoSource = new DirectoryInfo(watcher.Source);
                if (directoryInfoSource.Parent == null)
                {
                    NLogger.Info(
                        "Invalid Watcher:"
                        + Environment.NewLine
                        + "Action: {0}."
                        + Environment.NewLine
                        + "Source: {1}."
                        + Environment.NewLine
                        + "Destination: {2}."
                        + Environment.NewLine
                        + "Watcher Source folder:"
                        + Environment.NewLine
                        + "{1}"
                        + Environment.NewLine
                        + "is root folder."
                        + Environment.NewLine
                        + "MirrorFreezeCopy will not Mirror or Copy from this folder.",
                        watcher.Action,
                        watcher.Source,
                        watcher.Destination);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        private bool PostcheckWatchersExist(List<Watcher> listWatcher)
        {
            if (listWatcher != null && listWatcher.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool PostcheckIsDefinedAction(Watcher watcher)
        {
            bool isDefinedAction = false;

            foreach (WatcherAction watcherAction in (WatcherAction[])Enum.GetValues(typeof(WatcherAction)))
            {
                if (watcher.Action == watcherAction.ToString("g"))
                {
                    isDefinedAction = true;
                }
            }

            if (!isDefinedAction)
            {
                NLogger.Info(
                    "Invalid Watcher Action: {0} is not defined in MirrorFreezeCopy. This watcher will be skipped.",
                    watcher.Action);
            }

            return isDefinedAction;
        }

        private bool PostcheckSourceFolderExisted(Watcher watcher)
        {
            if (Directory.Exists(watcher.Source))
            {
                return true;
            }
            else
            {
                NLogger.Info(
                       "Invalid Watcher:"
                       + Environment.NewLine
                       + "Action: {0}."
                       + Environment.NewLine
                       + "Source: {1}."
                       + Environment.NewLine
                       + "Destination: {2}."
                       + Environment.NewLine
                       + "Watcher Source folder:"
                       + Environment.NewLine
                       + "{1}"
                       + Environment.NewLine
                       + "doesn't exist.",
                       watcher.Action,
                       watcher.Source,
                       watcher.Destination);
                return false;
            }
        }

        private bool PostcheckDestinationFolderCanBeCreated(Watcher watcher)
        {
            if (!Directory.Exists(watcher.Destination))
            {
                try
                {
                    Directory.CreateDirectory(watcher.Destination);
                }
                catch (NotSupportedException ex)
                {
                    NLogger.Error(ex, "Error while creating Destination folder.");
                    NLogger.Info(
                       "Invalid Watcher:"
                       + Environment.NewLine
                       + "Action: {0}."
                       + Environment.NewLine
                       + "Source: {1}."
                       + Environment.NewLine
                       + "Destination: {2}."
                       + Environment.NewLine
                       + "Watcher Destination folder:"
                       + Environment.NewLine
                       + "{2}"
                       + Environment.NewLine
                       + "can not be created.",
                       watcher.Action,
                       watcher.Source,
                       watcher.Destination);
                    return false;
                }

                return true;
            }
            else
            {
                return true;
            }
        }
    }
}