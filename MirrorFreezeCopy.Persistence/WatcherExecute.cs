// <copyright file="WatcherExecute.cs" company="Huy Tran">
// Copyright (c) Huy Tran. All rights reserved.
// </copyright>

namespace MirrorFreezeCopy.Persistence
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using MirrorFreezeCopy.Domain;
    using MirrorFreezeCopy.Domain.Enums;
    using MirrorFreezeCopy.Domain.Interfaces;

    /// <summary>
    /// Implementation of IWatcherExecute and IDisposable interfaces.
    /// </summary>
    public class WatcherExecute : IWatcherExecute, IDisposable
    {
        private static readonly NLog.Logger NLogger = NLog.LogManager.GetCurrentClassLogger();
        private int lineCount = 0;
        private int errorLineCount = 0;
        private Process commandlineProcess;
        private StringBuilder errorOutput = new StringBuilder();
        private StringBuilder output = new StringBuilder();
        private string commandLine;
        private string source;
        private string destination;
        private Watcher watcher;

        /// <summary>
        /// Execute command line.
        /// </summary>
        /// <param name="watcher"> Watcher object that contains neccessary information for to execute command line.</param>
        /// <param name="numberOfRetries"> Number of Retries on failed copies - default is 1 million.</param>
        /// <param name="interval"> Wait time between retries - default is 30 seconds.</param>
        public void Execute(Watcher watcher, int numberOfRetries = 1000000, int interval = 30)
        {
            this.watcher = watcher;

            if (!this.Precheck())
            {
                return;
            }

            this.source = this.CheckAndChangeForRootFolder(watcher.Source);
            this.destination = this.CheckAndChangeForRootFolder(watcher.Destination);

            if (watcher.Action == WatcherAction.Mirror.ToString("g"))
            {
                this.Mirror(watcher, numberOfRetries, interval);
            }

            if (watcher.Action == WatcherAction.Freeze.ToString("g"))
            {
                this.Freeze(watcher, numberOfRetries, interval);
            }

            if (watcher.Action == WatcherAction.Copy.ToString("g"))
            {
                this.Copy(watcher, numberOfRetries, interval);
            }
        }

        /// <summary>
        /// Dispose resources.
        /// </summary>
        public void Dispose()
        {
            if (this.commandlineProcess != null)
            {
                this.commandlineProcess.Dispose();
                this.commandlineProcess = null;
            }

            this.commandLine = string.Empty;
            this.errorOutput.Clear();
            this.output.Clear();
            this.lineCount = 0;
            this.errorLineCount = 0;
        }

        private void Mirror(Watcher watcher, int number, int interval)
        {
            if (Directory.Exists(watcher.Source))
            {
                // Mirror, meaning mirror copy from Source folder to Destination folder.
                this.commandLine = "ROBOCOPY /E /S /MIR /NFL /NDL /NS /NC /B /R:"
                    + number.ToString()
                    + " /W:"
                    + interval.ToString()
                    + @" """
                    + this.source
                    + @""" """
                    + this.destination
                    + @"""";
                this.CreateAndRunCommandLine();
            }
            else
            {
                NLogger.Info(
                    "Source folder with path: {0} doesn't exist." +
                    System.Environment.NewLine +
                    "Please check the config file.",
                    watcher.Source);
            }
        }

        private void Freeze(Watcher watcher, int number, int interval)
        {
            if (Directory.Exists(watcher.Source) && Directory.Exists(watcher.Destination))
            {
                // Freeze, meaning mirror copy from Destination folder to Source folder.
                this.commandLine = "ROBOCOPY /E /S /MIR /NFL /NDL /NS /NC /B /R:"
                    + number.ToString()
                    + " /W:"
                    + interval.ToString()
                    + @" """
                    + this.destination
                    + @""" """
                    + this.source
                    + @"""";
                this.CreateAndRunCommandLine();
            }
            else
            {
                NLogger.Info(
                    "Source folder: {0} and/or destination folder: {1} doesn't exist." +
                    System.Environment.NewLine +
                    "Please check the config file.",
                    watcher.Source,
                    argument2: watcher.Destination);
            }
        }

        private void Copy(Watcher watcher, int number, int interval)
        {
            if (Directory.Exists(watcher.Source))
            {
                // Copy, meaning copy only, without mirror, from Source folder to Destination folder.
                this.commandLine = "ROBOCOPY /E /S /NFL /NDL /NS /NC /B /R:"
                    + number.ToString()
                    + " /W:"
                    + interval.ToString()
                    + @" """
                    + this.source
                    + @""" """
                    + this.destination
                    + @"""";
                this.CreateAndRunCommandLine();
            }
            else
            {
                NLogger.Info(
                     "Source folder with path: {0} doesn't exist." +
                     System.Environment.NewLine +
                     "Please check the config file.",
                     watcher.Source);
            }
        }

        private void CreateAndRunCommandLine()
        {
            try
            {
                this.commandlineProcess = new Process();
                this.commandlineProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                this.commandlineProcess.StartInfo.CreateNoWindow = true;
                this.commandlineProcess.StartInfo.UseShellExecute = false;
                this.commandlineProcess.EnableRaisingEvents = false;
                this.commandlineProcess.StartInfo.RedirectStandardOutput = true;
                this.commandlineProcess.StartInfo.RedirectStandardError = true;
                this.commandlineProcess.OutputDataReceived += new DataReceivedEventHandler((sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        this.lineCount++;
                        this.output.Append("\n[" + this.lineCount + "]: " + e.Data);
                    }
                });
                this.commandlineProcess.ErrorDataReceived += new DataReceivedEventHandler((sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        this.errorLineCount++;
                        this.errorOutput.Append("\n[" + this.errorLineCount + "]: " + e.Data);
                    }
                });
                this.commandlineProcess.StartInfo.FileName = "cmd.exe";
                this.commandlineProcess.StartInfo.Arguments = "/c " + this.commandLine;
                this.commandlineProcess.Start();
                this.commandlineProcess.BeginOutputReadLine();
                this.commandlineProcess.BeginErrorReadLine();
                this.commandlineProcess.WaitForExit();
                if (this.output.Length != 0)
                {
                    NLogger.Info(this.output);
                }
                else
                {
                    NLogger.Info(
                        "Command line has been run with no output: "
                        + Environment.NewLine
                        + "{0}", this.commandLine);
                }
            }
            catch (Exception ex)
            {
                NLogger.Error(ex, "Error while running command line.");
                if (this.errorOutput.Length > 0)
                {
                    NLogger.Error("Error output: {0}", this.errorOutput);
                }
            }
            finally
            {
                this.Dispose();
            }
        }

        private string CheckAndChangeForRootFolder(string folderPath)
        {
            // Change if user's config is root folder, so that ROBOCOPY command line will not return error while executing.
            // For example, D:\ will be changed to D:
            folderPath = Path.GetFullPath(folderPath);

            if (folderPath.Length == 3)
            {
                return folderPath = folderPath.Remove(2);
            }
            else
            {
                return folderPath;
            }
        }

        private bool Precheck()
        {
            if (this.watcher == null)
            {
                NLogger.Info("Watcher is null.");
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}