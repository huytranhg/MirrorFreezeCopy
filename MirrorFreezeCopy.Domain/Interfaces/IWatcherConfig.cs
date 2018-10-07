// <copyright file="IWatcherConfig.cs" company="Huy Tran">
// Copyright (c) Huy Tran. All rights reserved.
// </copyright>

namespace MirrorFreezeCopy.Domain.Interfaces
{
    using System.Collections.Generic;
    using MirrorFreezeCopy.Domain.DTO;

    /// <summary>
    /// Interface to be implemented in persistence layer.
    /// </summary>
    public interface IWatcherConfig
    {
        /// <summary>
        /// Read RetryOption config from xml file.
        /// </summary>
        /// <param name="filePath"> Absolute path of xml config file</param>
        /// <returns>RetryOptionConfigDto object</returns>
        RetryOptionConfigDto ReadRetryOption(string filePath);

        /// <summary>
        /// Read List of Watchers config from xml file.
        /// </summary>
        /// <param name="filePath"> Absolute path of xml config file</param>
        /// <returns>List of WatcherConfigDto objects</returns>
        List<WatcherConfigDto> ReadWatchers(string filePath);

        /// <summary>
        /// Write a sample config to xml file.
        /// </summary>
        /// <param name="filePath"> Absolute path of xml config file</param>
        void WriteSampleConfig(string filePath);
    }
}