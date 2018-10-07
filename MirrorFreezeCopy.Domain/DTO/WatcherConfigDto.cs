// <copyright file="WatcherConfigDto.cs" company="Huy Tran">
// Copyright (c) Huy Tran. All rights reserved.
// </copyright>

namespace MirrorFreezeCopy.Domain.DTO
{
    /// <summary>
    /// Data transfer object of Watcher object.
    /// </summary>
    public class WatcherConfigDto
    {
        /// <summary>
        /// Gets or sets action
        /// </summary>
        public string Action { get; set; }

        /// <summary>
        /// Gets or sets source
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// Gets or sets destination
        /// </summary>
        public string Destination { get; set; }
    }
}