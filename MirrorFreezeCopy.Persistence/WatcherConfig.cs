// <copyright file="WatcherConfig.cs" company="Huy Tran">
// Copyright (c) Huy Tran. All rights reserved.
// </copyright>

namespace MirrorFreezeCopy.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Xml.Serialization;
    using MirrorFreezeCopy.Domain.DTO;
    using MirrorFreezeCopy.Domain.Enums;
    using MirrorFreezeCopy.Domain.Interfaces;

    /// <summary>
    /// Implementation of IWatcherConfig interface.
    /// </summary>
    public class WatcherConfig : IWatcherConfig
    {
        private static readonly NLog.Logger NLogger = NLog.LogManager.GetCurrentClassLogger();
        private static readonly string ConfigFileName = "MirrorFreezeCopy_Config.xml";
        private static readonly string ConfigFilePath =
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                "MirrorFreezeCopy",
                ConfigFileName);

        /// <summary>
        /// Read RetryOption config from xml file.
        /// </summary>
        /// <param name="filePath"> Absolute path of xml config file</param>
        /// <returns>RetryOptionConfigDto object</returns>
        public RetryOptionConfigDto ReadRetryOption(string filePath)
        {
            MirrorFreezeCopyConfigDto mirrorFreezeCopyConfigDto;
            XmlSerializer reader =
            new XmlSerializer(typeof(MirrorFreezeCopyConfigDto));

            // If the XML document has been altered with unknown
            // nodes or attributes, handles them with the
            // UnknownNode and UnknownAttribute events.
            reader.UnknownNode += new
            XmlNodeEventHandler(this.Serializer_UnknownNode);
            reader.UnknownAttribute += new
            XmlAttributeEventHandler(this.Serializer_UnknownAttribute);

            try
            {
                using (FileStream file = new FileStream(filePath, FileMode.Open))
                {
                    mirrorFreezeCopyConfigDto = (MirrorFreezeCopyConfigDto)reader.Deserialize(file);
                    file.Close();
                }

                return mirrorFreezeCopyConfigDto.RetryOptionConfigDto;
            }
            catch (InvalidOperationException ioEx)
            {
                NLogger.Error(
                    ioEx,
                    "The config file's RetryOption could not be interpreted." + Environment.NewLine + "Please check the config file.");
                return null;
            }
            catch (Exception ex)
            {
                NLogger.Fatal(ex, "The config file could not be read.");
                reader = null;
                return null;
            }
        }

        /// <summary>
        /// Read Watchers config from xml file.
        /// </summary>
        /// <param name="filePath"> Absolute path of xml config file</param>
        /// <returns>List of WatcherConfigDto objects</returns>
        public List<WatcherConfigDto> ReadWatchers(string filePath)
        {
            MirrorFreezeCopyConfigDto mirrorFreezeCopyConfigDto;
            XmlSerializer reader =
            new XmlSerializer(typeof(MirrorFreezeCopyConfigDto));

            // If the XML document has been altered with unknown
            // nodes or attributes, handles them with the
            // UnknownNode and UnknownAttribute events.
            reader.UnknownNode += new
            XmlNodeEventHandler(this.Serializer_UnknownNode);
            reader.UnknownAttribute += new
            XmlAttributeEventHandler(this.Serializer_UnknownAttribute);

            try
            {
                using (FileStream file = new FileStream(filePath, FileMode.Open))
                {
                    mirrorFreezeCopyConfigDto = (MirrorFreezeCopyConfigDto)reader.Deserialize(file);
                    file.Close();
                }

                return mirrorFreezeCopyConfigDto.ListWatcherConfigDto;
            }
            catch (InvalidOperationException ioEx)
            {
                NLogger.Error(
                    ioEx,
                    "The config file's Watchers could not be interpreted." + Environment.NewLine + "Please check the config file.");
                return null;
            }
            catch (Exception ex)
            {
                NLogger.Fatal(ex, "The config file could not be read.");
                reader = null;
                return null;
            }
        }

        /// <summary>
        /// Write sample config to xml file.
        /// </summary>
        /// <param name="filePath"> Absolute path of xml config file</param>
        public void WriteSampleConfig(string filePath)
        {
            try
            {
                XmlSerializer serializer =
                    new XmlSerializer(typeof(MirrorFreezeCopyConfigDto));

                MirrorFreezeCopyConfigDto mirrorFreezeCopyConfigDto = new MirrorFreezeCopyConfigDto();

                // ROBOCOPY default is retry for 1 million times, per 30 seconds.
                mirrorFreezeCopyConfigDto.RetryOptionConfigDto = new RetryOptionConfigDto()
                { NumberOfRetries = 1000000, Interval = 30 };
                mirrorFreezeCopyConfigDto.ListWatcherConfigDto = new List<WatcherConfigDto>
                {
                    new WatcherConfigDto
                    {
                        Action = "Mirror",
                        Source = Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                            "MirrorFreezeCopy",
                            "Sample Mirror Source"),
                        Destination = Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                            "MirrorFreezeCopy",
                            "Sample Mirror Destination"),
                    }
                };

                this.CreateDirectory(mirrorFreezeCopyConfigDto.ListWatcherConfigDto[0].Source);
                this.CreateDirectory(mirrorFreezeCopyConfigDto.ListWatcherConfigDto[0].Destination);
                this.CreateDirectory(Path.GetDirectoryName(ConfigFilePath));

                using (TextWriter writer = new StreamWriter(filePath, false, Encoding.UTF8))
                {
                    serializer.Serialize(writer, mirrorFreezeCopyConfigDto);
                    writer.Close();
                }
            }
            catch (Exception ex)
            {
                NLogger.Error(ex, "Error while creating sample config file.");
            }
        }

        private void Serializer_UnknownNode(
            object sender, XmlNodeEventArgs e)
        {
            NLogger.Info("Unknown Node:" + e.Name + "\t" + e.Text);
        }

        private void Serializer_UnknownAttribute(
            object sender, XmlAttributeEventArgs e)
        {
            System.Xml.XmlAttribute attr = e.Attr;
            NLogger.Info("Unknown attribute " +
            attr.Name + "='" + attr.Value + "'");
        }

        private void CreateDirectory(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
        }
    }
}
