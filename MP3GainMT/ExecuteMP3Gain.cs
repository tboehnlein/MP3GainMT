using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MP3GainMT
{
    public class ExecuteMP3Gain
    {
        private MP3GainFile activeFile = null;
        private List<string> sortedFiles = new List<string>();
        private TimeCheck progressTimeCheck = new TimeCheck(5);
        private int filesFinished = 0;

        private string FilePrefix { get; set; } = string.Empty;
        public string EndingText { get; private set; }
        public string ActionName { get; set; } = string.Empty;
        public BackgroundWorker Worker { get; private set; }
        public Dictionary<string, MP3GainFile> Files { get; set; } = new Dictionary<string, MP3GainFile>();

        public string FolderPath { get; set; } = string.Empty;

        public string Executable { get; set; }
        public string Arguments { get; set; }

        public string FolderName => Path.GetDirectoryName(FolderPath);


        public Process Process { get; set; }

        public ExecuteMP3Gain(string executable, string arguments, Dictionary<string, MP3GainFile> files, string folder, string actionName, BackgroundWorker worker, string fileOutputPrefix, string endingOutputText)
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

            Debug.WriteLine($"STARTED {this.ActionName} FOR {this.FolderName}");

            this.Process.WaitForExit();

            this.Process.OutputDataReceived -= Process_OutputDataReceived;
            this.Process.ErrorDataReceived -= Process_ErrorDataReceived;

            Debug.WriteLine($"FINISHED {this.ActionName} FOR {this.FolderName}");
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

        public virtual void UpdateProgress(DataReceivedEventArgs e)
        {
            if (e.Data.Contains("%"))
            {
                var items = e.Data.Split('%');                
                var percent = Convert.ToInt32(items[0].Trim());

                if (progressTimeCheck.CheckTime())
                {
                    Debug.WriteLine($"PROGRESS {this.ActionName}: {percent}% {activeFile.FilePath}");
                    UpdateFileProgress(percent);
                }
            }
        }

        private void UpdateFileProgress(int percent)
        {
            this.activeFile.Progress = percent;
            int overallProgress = GetOverallProgress(this.activeFile.Progress);
            this.Worker.ReportProgress(overallProgress, new FileProgress(this.activeFile, this.activeFile.Progress));
        }

        private int GetOverallProgress(int filePercent)
        {
            var oneFilePercent = 1.0 / this.sortedFiles.Count;
            var fileProgress = oneFilePercent * (filePercent / 100.0);
            var finishedProgress = (double)this.filesFinished / this.sortedFiles.Count;

            return Convert.ToInt32((finishedProgress + fileProgress) * 100.0);
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
                    Debug.WriteLine($"FINISHED {this.ActionName}: {activeFile.FilePath}");
                    this.activeFile.Progress = 100;
                    this.activeFile.UpdateTags();
                    int overallProgress = GetOverallProgress(100);
                    this.Worker.ReportProgress(overallProgress, new FileProgress(this.activeFile, this.activeFile.Progress));
                    filesFinished++;
                }
            }
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

        public virtual void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            //Debug.WriteLine($"OUTPUT \"{e.Data}\"");
        }
    }
}
