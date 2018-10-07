// <copyright file="IWatcherExecute.cs" company="Huy Tran">
// Copyright (c) Huy Tran. All rights reserved.
// </copyright>

namespace MirrorFreezeCopy.Domain.Interfaces
{
    using System.Collections.Generic;
    using MirrorFreezeCopy.Domain.DTO;

    /// <summary>
    /// Interface to be implemented in persistence layer.
    /// </summary>
    public interface IWatcherExecute
    {
        /// <summary>
        /// Execute command line.
        /// </summary>
        /// <param name="watcher"> Watcher object contains information neccessary for command line execution.</param>
        /// <param name="numberOfRetries"> NumberOfRetries on failed copies - default is 1 million.</param>
        /// <param name="interval"> Wait time between retries - default is 30 seconds.</param>
        void Execute(Watcher watcher, int numberOfRetries = 1000000, int interval = 30);
    }
}