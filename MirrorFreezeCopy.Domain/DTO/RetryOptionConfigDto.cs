// <copyright file="RetryOptionConfigDto.cs" company="Huy Tran">
// Copyright (c) Huy Tran. All rights reserved.
// </copyright>

namespace MirrorFreezeCopy.Domain.DTO
{
    /// <summary>
    /// Data transfer object of RetryOption object.
    /// </summary>
    public class RetryOptionConfigDto
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