// <copyright file="Watcher.cs" company="Huy Tran">
// Copyright (c) Huy Tran. All rights reserved.
// </copyright>

namespace MirrorFreezeCopy.Domain
{
    /// <summary>
    /// Watcher object, will be mapped wil WatcherConfigDto object.
    /// </summary>
    public class Watcher
    {
        /// <summary>
        ///  Gets or sets action
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