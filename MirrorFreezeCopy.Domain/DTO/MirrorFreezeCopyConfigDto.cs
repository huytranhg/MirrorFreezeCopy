// <copyright file="MirrorFreezeCopyConfigDto.cs" company="Huy Tran">
// Copyright (c) Huy Tran. All rights reserved.
// </copyright>

namespace MirrorFreezeCopy.Domain.DTO
{
    using System.Collections.Generic;
    using System.Xml;
    using System.Xml.Serialization;

    /// <summary>
    /// Data transfer object of MirrorFreezeCopyConfig object.
    /// </summary>
    [XmlRoot(
        "MirrorFreezeCopy",
        IsNullable = false)]
    public class MirrorFreezeCopyConfigDto
    {
        /// <summary>
        /// Gets or sets RetryOptionDto
        /// </summary>
        [XmlElement("RetryOption", IsNullable = true)]
        public RetryOptionConfigDto RetryOptionConfigDto { get; set; }

        /// <summary>
        /// Gets or sets RetryOptionDto
        /// </summary>
        [XmlArray("Watchers")]
        [XmlArrayItem("WatcherConfig")]
        public List<WatcherConfigDto> ListWatcherConfigDto { get; set; }
    }
}
