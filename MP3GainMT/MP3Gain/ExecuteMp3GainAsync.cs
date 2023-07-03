﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace MP3GainMT.MP3Gain
{
    public class ExecuteMp3GainAsync
    {
        protected Mp3File activeFile = null;
        protected int filesFinished = 0;
        protected TimeCheck progressTimeCheck = new TimeCheck(5);
        protected List<string> sortedFiles = new List<string>();

        public ExecuteMp3GainAsync(string executable,
                              string arguments,
                              Dictionary<string, Mp3File> files,
                              string folder,
                              string actionName,
                              BackgroundWorker worker,
                              string fileOutputPrefix,
                              string endingOutputText)
        {
            this.Executable = executable;
            this.Arguments = arguments;
            this.Files = files;
            this.FolderPath = folder;
            this.ActionName = actionName;
            this.Worker = worker;
            this.FilePrefix = fileOutputPrefix;
            this.EndingText = endingOutputText;
        }

        public string ActionName { get; set; } = string.Empty;
        public string Arguments { get; set; }
        public string EndingText { get; private set; }
        public string Executable { get; set; }
        public Dictionary<string, Mp3File> Files { get; set; } = new Dictionary<string, Mp3File>();
        public string FolderName => Path.GetDirectoryName(FolderPath);
        public string FolderPath { get; set; } = string.Empty;
        public Process Process { get; set; }
        public BackgroundWorker Worker { get; private set; }
        protected string FilePrefix { get; set; } = string.Empty;

        public virtual void Execute()
        {
            this.sortedFiles = this.Files.Select(x => x.Value.FilePath).ToList();
            sortedFiles.Sort();

            var parameters = $"{this.Arguments} \"{Path.Combine(FolderPath, "*.mp3")}\"";
            var processStart = new ProcessStartInfo(Executable, parameters);
            processStart.UseShellExecute = false;
            processStart.RedirectStandardOutput = true;
            processStart.RedirectStandardError = true;
            processStart.CreateNoWindow = true;

            this.Process = new Process();

            this.Process.StartInfo = processStart;

            this.Process.OutputDataReceived += Process_OutputDataReceived;
            this.Process.ErrorDataReceived += Process_ErrorDataReceived;

            // assume first file will be ran first in *.mp3
            this.activeFile = this.Files[sortedFiles.First()];

            this.Process.Start();

            this.Process.BeginOutputReadLine();
            this.Process.BeginErrorReadLine();

            //Debug.WriteLine($"STARTED {this.ActionName} FOR {this.FolderName}");

            this.Process.WaitForExit();

            this.Process.OutputDataReceived -= Process_OutputDataReceived;
            this.Process.ErrorDataReceived -= Process_ErrorDataReceived;

            //Debug.WriteLine($"FINISHED {this.ActionName} FOR {this.FolderName}");
        }

        public virtual void ExtractActiveFile(DataReceivedEventArgs e)
        {
            if (e.Data.Contains(this.FilePrefix))
            {
                var toIndex = e.Data.IndexOf(" to ");
                var endToIndex = toIndex + 4;

                var fileStartString = e.Data.Substring(endToIndex);

                if (fileStartString.Contains("..."))
                {
                    var fileEndIndex = fileStartString.IndexOf("...");
                    var fileString = fileStartString.Substring(0, fileEndIndex);
                    if (this.sortedFiles.Contains(fileString))
                    {
                        this.activeFile = this.Files[fileString];
                    }
                }
            }
        }

        public virtual void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!String.IsNullOrEmpty(e.Data))
            {
                ExtractActiveFile(e);
                UpdateProgress(e);
                ProcessFileEnding(e);
            }
        }

        public virtual void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            //Debug.WriteLine($"OUTPUT \"{e.Data}\"");
        }

        public virtual void ProcessFileEnding(DataReceivedEventArgs e)
        {
            if (e.Data == this.EndingText)
            {
                while (this.activeFile.IsFileLocked())
                {
                    System.Threading.Thread.Sleep(25);
                }

                if (progressTimeCheck.CheckTime(true))
                {
                    //Debug.WriteLine($"FINISHED {this.ActionName}: {activeFile.FilePath}");
                    this.activeFile.Progress = 100;
                    this.activeFile.UpdateTags();
                    int overallProgress = GetOverallProgress(100);
                    this.Worker.ReportProgress(overallProgress, new FileProgress(this.activeFile, this.activeFile.Progress));
                    filesFinished++;
                }
            }
        }

        public virtual void UpdateProgress(DataReceivedEventArgs e)
        {
            if (e.Data.Contains("%"))
            {
                var items = e.Data.Split('%');
                if (int.TryParse(items[0].Trim(), out int percent) && progressTimeCheck.CheckTime())
                {
                    //Debug.WriteLine($"PROGRESS {this.ActionName}: {percent}% {activeFile.FilePath}");
                    UpdateFileProgress(percent);
                }
            }
        }

        protected int GetOverallProgress(int filePercent)
        {
            var oneFilePercent = 1.0 / this.sortedFiles.Count;
            var fileProgress = oneFilePercent * (filePercent / 100.0);
            var finishedProgress = (double)this.filesFinished / this.sortedFiles.Count;

            return Convert.ToInt32((finishedProgress + fileProgress) * 100.0);
        }

        protected void UpdateFileProgress(int percent)
        {
            this.activeFile.Progress = percent;
            int overallProgress = GetOverallProgress(this.activeFile.Progress);
            this.Worker.ReportProgress(overallProgress, new FileProgress(this.activeFile, this.activeFile.Progress));
        }
    }
}