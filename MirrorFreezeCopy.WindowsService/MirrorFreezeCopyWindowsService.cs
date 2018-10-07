// <copyright file="MirrorFreezeCopyWindowsService.cs" company="Huy Tran">
// Copyright (c) Huy Tran. All rights reserved.
// </copyright>

namespace MirrorFreezeCopy
{
    using System;
    using System.Linq;
    using System.ServiceProcess;

    /// <summary>
    /// MirrorFreezeCopy Windows Service.
    /// </summary>
    public partial class MirrorFreezeCopyWindowsService : ServiceBase
    {
        private static readonly NLog.Logger NLogger = NLog.LogManager.GetCurrentClassLogger();
        private System.Diagnostics.EventLog eventLog;
        private MirrorFreezeCopyService mirrorFreezeCopyService;

        /// <summary>
        /// Initializes a new instance of the <see cref="MirrorFreezeCopyWindowsService"/> class.
        /// </summary>
        /// <param name="args"> Arguments from command line.</param>
        public MirrorFreezeCopyWindowsService(string[] args)
        {
            this.InitializeComponent();

            string eventSourceName = "MirrorFreezeCopySource";
            string logName = "MirrorFreezeCopyLog";

            if (args.Count() > 0)
            {
                eventSourceName = args[0];
            }

            if (args.Count() > 1)
            {
                logName = args[1];
            }

            this.eventLog = new System.Diagnostics.EventLog();
            this.eventLog.Source = eventSourceName;
            this.eventLog.Log = logName;
        }

        /// <summary>
        /// Helper method to debug OnStart, OnStop, OnContinue by running as a Console Application, instead of Windows Service.
        /// </summary>
        /// <param name="args"> Arguments from command line</param>
        internal void TestStartupAndStop(string[] args)
        {
            this.OnStart(args);
            Console.ReadLine();
            this.OnStop();
            Console.ReadLine();
            this.OnContinue();
        }

        /// <summary>
        /// Run when MirrorFreezeCopy Windows Service starts.
        /// </summary>
        /// <param name="args"> Arguments from command line</param>
        protected override void OnStart(string[] args)
        {
            this.eventLog.WriteEntry("MirrorFreezeCopy Windows Service is starting...");
            NLogger.Info("MirrorFreezeCopy Windows Service is starting...");
            this.mirrorFreezeCopyService = new MirrorFreezeCopyService();
            this.mirrorFreezeCopyService.Start();
            this.eventLog.WriteEntry("MirrorFreezeCopy Windows Service started.");
            NLogger.Info("MirrorFreezeCopy Windows Service started.");
        }

        /// <summary>
        /// Run when MirrorFreezeCopy Windows Service stops.
        /// </summary>
        protected override void OnStop()
        {
            this.eventLog.WriteEntry("MirrorFreezeCopy Windows Service is stopping...");
            NLogger.Info("MirrorFreezeCopy Windows Service is stopping...");

            if (this.mirrorFreezeCopyService != null)
            {
                this.mirrorFreezeCopyService.Dispose();
                this.mirrorFreezeCopyService = null;
            }

            this.eventLog.WriteEntry("MirrorFreezeCopy Windows Service stopped.");
            NLogger.Info("MirrorFreezeCopy Windows Service stopped.");
        }

        /// <summary>
        /// Run when MirrorFreezeCopy Windows Service continues.
        /// </summary>
        protected override void OnContinue()
        {
            this.eventLog.WriteEntry("MirrorFreezeCopy Windows Service is continuing...");
            NLogger.Info("MirrorFreezeCopy Windows Service is continuing...");

            this.mirrorFreezeCopyService = new MirrorFreezeCopyService();
            this.mirrorFreezeCopyService.Start();

            this.eventLog.WriteEntry("MirrorFreezeCopy Windows Service continued.");
            NLogger.Info("MirrorFreezeCopy Windows Service continued.");
        }
    }
}