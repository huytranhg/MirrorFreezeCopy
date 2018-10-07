// <copyright file="Program.cs" company="Huy Tran">
// Copyright (c) Huy Tran. All rights reserved.
// </copyright>

namespace MirrorFreezeCopy
{
    using System;
    using System.Collections.Generic;
    using System.Configuration.Install;
    using System.Reflection;
    using System.ServiceProcess;
    using AutoMapper;
    using MirrorFreezeCopy.Domain;
    using MirrorFreezeCopy.Domain.DTO;
    using NLog;

    /// <summary>
    /// Main startup class of MirrorFreezeCopy Windows Service.
    /// </summary>
    public static class Program
    {
        private static readonly NLog.Logger NLogger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// <param name="args"> Array of parameters passed from command line</param>
        public static void Main(string[] args)
        {
            AutomapperInitialize();

            // Below code is used to install MirrorFreezeCopy if getting --install parameter from command line.
            if (Environment.UserInteractive)
            {
                string parameter = string.Concat(args);
                switch (parameter)
                {
                    case "--install":
                        ManagedInstallerClass.InstallHelper(new string[] { Assembly.GetExecutingAssembly().Location });
                        break;
                    case "--uninstall":
                        ManagedInstallerClass.InstallHelper(new string[] { "/u", Assembly.GetExecutingAssembly().Location });
                        break;
                }

                // The below two lines is used for testing Windows Service Start and Stop as console's method.
                // Reference link: https://docs.microsoft.com/en-us/dotnet/framework/windows-services/how-to-debug-windows-service-applications
                // Note that to run in debug mode without exeception, it's required Adminitrator permission, in order for Event Log to work.
                //MirrorFreezeCopyWindowsService service1 = new MirrorFreezeCopyWindowsService(args);
                //service1.TestStartupAndStop(args);
            }
            else
            {
                // This is for MirrorFreezeCopy Windows Service to run.
                ServiceBase[] servicesToRun = new ServiceBase[] { new MirrorFreezeCopyWindowsService(args) };
                ServiceBase.Run(servicesToRun);
            }
        }

        /// <summary>
        /// Initilize Automapper to map between WatcherConfigDto data transfer object and Watcher object.
        /// </summary>
        private static void AutomapperInitialize()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<RetryOptionConfigDto, RetryOption>();
                cfg.CreateMap<WatcherConfigDto, Watcher>();
            });
        }
    }
}