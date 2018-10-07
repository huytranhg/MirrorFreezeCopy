// <copyright file="IWindowsFileSystemWatcher.cs" company="Huy Tran">
// Copyright (c) Huy Tran. All rights reserved.
// </copyright>

namespace MirrorFreezeCopy.Domain.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Interface to be implemented in persistence layer.
    /// </summary>
    public interface IWindowsFileSystemWatcher
    {
        /// <summary>
        /// Start file watcher.
        /// </summary>
        /// <param name="watcher"> Watcher object contains necssary information for file watcher to run.</param>
        void Start(Watcher watcher);
    }
}
