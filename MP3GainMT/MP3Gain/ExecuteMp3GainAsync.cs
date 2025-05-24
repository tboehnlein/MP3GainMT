// Copyright (c) 2025 Thomas Boehnlein
// 
// This software is provided 'as-is', without any express or implied
// warranty. In no event will the authors be held liable for any damages
// arising from the use of this software.
// 
// Permission is granted to anyone to use this software for any purpose,
// including commercial applications, and to alter it and redistribute it
// freely, subject to the following restrictions:
// 
// 1. The origin of this software must not be misrepresented; you must not
//    claim that you wrote the original software. If you use this software
//    in a product, an acknowledgment in the product documentation would be
//    appreciated but is not required.
// 2. Altered source versions must be plainly marked as such, and must not be
//    misrepresented as being the original software.
// 3. This notice may not be removed or altered from any source distribution.

using System;
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
        private Dictionary<string, string> fileLookUp;
        protected List<string> sortedFiles = new List<string>();
        private readonly Helpers.IFileRenamer _fileRenamer;
        private readonly Func<Interfaces.IProcess> _processFactory; // Added IProcess factory
        protected Interfaces.IProcess Process { get; set; } // Changed type to IProcess

        public ExecuteMp3GainAsync(string executable,
                              string arguments,
                              Dictionary<string, Mp3File> files,
                              Helpers.IFileRenamer fileRenamer,
                              Func<Interfaces.IProcess> processFactory, // Added IProcess factory param
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
            this._fileRenamer = fileRenamer ?? throw new ArgumentNullException(nameof(fileRenamer));
            this._processFactory = processFactory ?? throw new ArgumentNullException(nameof(processFactory)); // Store IProcess factory
        }

        public string ActionName { get; set; } = string.Empty;
        public string Arguments { get; set; }
        public string EndingText { get; private set; }
        public string Executable { get; set; }
        public Dictionary<string, Mp3File> Files { get; set; } = new Dictionary<string, Mp3File>();
        public string FolderName => Path.GetDirectoryName(FolderPath); // Should be Path.GetFileName
        public string FolderPath { get; set; } = string.Empty;
        // Process property is now above, typed as IProcess
        public BackgroundWorker Worker { get; private set; }
        protected string FilePrefix { get; set; } = string.Empty;

        public virtual void Execute()
        {
            //TODO: rename all of the files with a lookup dictionary to rename back to original
            // names at the end of the process

            this.fileLookUp = new Dictionary<string, string>();

            this.sortedFiles = this.Files.Select(x => x.Value.FilePath).ToList();
            sortedFiles.Sort();

            // Use injected _fileRenamer
            bool renameSuccess = _fileRenamer.RandomlyRenameFiles(this.fileLookUp, this.sortedFiles);

            if (!renameSuccess)
            {
                // Attempt to undo with _fileRenamer if initial rename failed partway
                _fileRenamer.UndoRandomlyRenameFiles(this.fileLookUp);
                return;
            }

            this.sortedFiles = this.fileLookUp.Keys.ToList();

            var parameters = $"{this.Arguments} \"{Path.Combine(FolderPath, "*.mp3")}\"";
            var processStart = new ProcessStartInfo(Executable, parameters);
            processStart.UseShellExecute = false;
            processStart.RedirectStandardOutput = true;
            processStart.RedirectStandardError = true;
            processStart.CreateNoWindow = true;

            this.Process = _processFactory(); // Use the factory
            this.Process.StartInfo = processStart;
            // EnableRaisingEvents should be true to receive events
            this.Process.EnableRaisingEvents = true; 

            this.Process.OutputDataReceived += Process_OutputDataReceived;
            this.Process.ErrorDataReceived += Process_ErrorDataReceived;

            // assume first file will be ran first in *.mp3
            this.activeFile = this.Files[this.fileLookUp[sortedFiles.First()]];

            this.Process.Start();

            this.Process.BeginOutputReadLine();
            this.Process.BeginErrorReadLine();

            //Debug.WriteLine($"STARTED {this.ActionName} FOR {this.FolderName}");

            this.Process.WaitForExit();

            this.Process.OutputDataReceived -= Process_OutputDataReceived;
            this.Process.ErrorDataReceived -= Process_ErrorDataReceived;
            
            // Use injected _fileRenamer
            _fileRenamer.UndoRandomlyRenameFiles(this.fileLookUp);

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
                    var fileEndIndex = fileStartString.LastIndexOf("...");
                    var fileString = fileStartString.Substring(0, fileEndIndex);
                    if (this.sortedFiles.Contains(fileString))
                    {
                        this.activeFile = this.Files[this.fileLookUp[fileString]];
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
                //System.Threading.Thread.Sleep(50);

                //while (this.activeFile.IsFileLocked())
                //{
                //    System.Threading.Thread.Sleep(50);
                //}

                if (progressTimeCheck.CheckTime(true))
                {
                    //Debug.WriteLine($"FINISHED {this.ActionName}: {activeFile.FilePath}");
                    this.activeFile.Progress = 100;
                    this.activeFile.FlagUpdateTags();
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