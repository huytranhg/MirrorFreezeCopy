// <copyright file="MirrorFreezeCopyConfig.cs" company="Huy Tran">
// Copyright (c) Huy Tran. All rights reserved.
// </copyright>

namespace MirrorFreezeCopy.Domain
{
    using System.Collections.Generic;

    /// <summary>
    /// MirrorFreezeCopyConfig object, will be mapped with MirrorFreezeCopyConfigDto object.
    /// </summary>
    public class MirrorFreezeCopyConfig
    {
        /// <summary>
        /// Gets or sets RetryOption.
        /// </summary>
        public RetryOption RetryOption { get; set; }

        /// <summary>
        /// Gets or sets ListWatcher.
        /// </summary>
        public List<Watcher> ListWatcher { get; set; }
    }
}
