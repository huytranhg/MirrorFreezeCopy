// <copyright file="RetryOption.cs" company="Huy Tran">
// Copyright (c) Huy Tran. All rights reserved.
// </copyright>

namespace MirrorFreezeCopy.Domain
{
    /// <summary>
    /// RetryOption object, will be mapped with RetryOptionDto object.
    /// </summary>
    public class RetryOption
    {
        /// <summary>
        /// Gets or sets NumberOfRetries
        /// </summary>
        public int NumberOfRetries { get; set; }

        /// <summary>
        /// Gets or sets Interval
        /// </summary>
        public int Interval { get; set; }
    }
}