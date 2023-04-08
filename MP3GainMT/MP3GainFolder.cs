﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace MP3GainMT
{
    public class MP3GainFolder
    {
        private StringBuilder sortOutput;
        private StringBuilder sortError;
        private MP3GainFile activeFile;
        private BackgroundWorker worker;

        public ExecuteMP3Gain UndoGainExecution { get; private set; }

        private List<string> sortedFiles;
        private static DateTime lastWrite = DateTime.Now;

        public event EventHandler<MP3GainFile> FoundFile;

        public event EventHandler<MP3GainFile> ChangedFile;

        public Dictionary<string, MP3GainFile> Files { get; set; } = new Dictionary<string, MP3GainFile>();

        public string FolderPath { get; set; } = string.Empty;

        public string FolderName { get; set; } = string.Empty;

        public int SuggestedGain { get; set; } = 0;

        public double DBOffset { get; set; } = 0.0;

        public List<string> MP3Files
        {
            get
            {
                return Files.Select(x => x.Value.FilePath).ToList();
            }
        }

        public string AnalaysisOutput { get; set; } = string.Empty;
        public string RunOutput { get; private set; } = string.Empty;
        public string GainOutput { get; private set; } = string.Empty;
        public List<string> AnalysisLines { get; private set; } = new List<string>();
        public List<string> GainLines { get; private set; } = new List<string>();
        public ExecuteMP3Gain ApplyGainExecution { get; private set; }

        public MP3GainFolder(string path)
        {
            this.FolderPath = path;
            this.FolderName = Path.GetFileName(path);
        }

        public void SearchFolder()
        {
            this.FindFiles(this.FolderPath);
        }

        private void FindFiles(string folderPath)
        {
            var files = Directory.GetFiles(folderPath, "*.mp3", SearchOption.TopDirectoryOnly);

            foreach (var file in files)
            {
                if (File.Exists(file))
                {
                    var mp3File = new MP3GainFile(file);

                    this.Files.Add(mp3File.FilePath, mp3File);

                    this.RaiseFoundFile(mp3File);
                }
            }
        }

        internal void ApplyGainFolder(string executable, BackgroundWorker worker)
        {
            this.worker = worker;
            this.ExecuteApplyGain(executable);
        }

        internal void ProcessFiles(string executable, BackgroundWorker worker)
        {
            this.worker = worker;
            this.RunAnalysis(executable);
        }

        private void RunApplyGain(string executable)
        {
            this.sortedFiles = this.Files.Select(x => x.Value.FilePath).ToList();
            sortedFiles.Sort();

            /*foreach (var file in sortedFiles)
            {
                var parameters = $"/o /g {this.SuggestedGain} \"{file}\"";
                var gainStart = new ProcessStartInfo(executable, parameters);
                gainStart.UseShellExecute = false;
                gainStart.RedirectStandardOutput = false;
                gainStart.RedirectStandardError = true;
                gainStart.CreateNoWindow = true;
                var gainProcess = new Process();
                gainProcess.StartInfo = gainStart;
                //this.sortOutput = new StringBuilder();
                //this.sortError = new StringBuilder();

                //gainProcess.OutputDataReceived += GainProcess_OutputDataReceived;
                gainProcess.ErrorDataReceived += GainProcess_ErrorDataReceived;

                this.activeFile = this.Files[file];
                gainProcess.Start();
                //gainProcess.BeginOutputReadLine();
                gainProcess.BeginErrorReadLine();
                Debug.WriteLine($"STARTED ANALYSIS FOR {file}");
                gainProcess.WaitForExit();
                Debug.WriteLine($"FINISHED ANALYSIS FOR {file}");
            }*/

            var parameters = $"/o /g {this.SuggestedGain} \"{Path.Combine(FolderPath, "*.mp3")}\"";
            var gainStart = new ProcessStartInfo(executable, parameters);
            gainStart.UseShellExecute = false;
            gainStart.RedirectStandardOutput = true;
            gainStart.RedirectStandardError = true;
            gainStart.CreateNoWindow = true;

            var gainProcess = new Process();

            gainProcess.StartInfo = gainStart;

            //this.sortOutput = new StringBuilder();
            //this.sortError = new StringBuilder();

            gainProcess.OutputDataReceived += GainProcess_OutputDataReceived;
            gainProcess.ErrorDataReceived += GainProcess_ErrorDataReceived;


            this.activeFile = this.Files[sortedFiles.First()];

            gainProcess.Start();

            gainProcess.BeginOutputReadLine();
            gainProcess.BeginErrorReadLine();

            Debug.WriteLine($"STARTED ANALYSIS FOR {this.FolderName}");

            gainProcess.WaitForExit();

            gainProcess.OutputDataReceived -= GainProcess_OutputDataReceived;
            gainProcess.ErrorDataReceived -= GainProcess_ErrorDataReceived;
        }

        private void GainProcess_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            //Debug.Write(e.Data);

            if (!String.IsNullOrEmpty(e.Data))
            {
                if (e.Data.Contains("Applying gain"))
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

                // Add the text to the collected output.
                //sortError.Append(e.Data);
                //Debug.WriteLine(e.Data);

                //Debug.WriteLine(e.Data);

                if (e.Data.Contains("%"))
                {
                    var items = e.Data.Split('%');
                    var percentItems = items[0].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    var percent = Convert.ToInt32(percentItems[0]);

                    if (percent != this.activeFile.Progress)
                    {
                        this.activeFile.Progress = percent;
                        ////Debug.WriteLine($"PROGRESS: {percent} {this.activeFile.FilePath}");
                        this.worker.ReportProgress(this.activeFile.Progress, this.activeFile);
                        //Application.DoEvents();
                        this.RaiseChangedFile(this.activeFile);
                        lastWrite = DateTime.Now;
                    }
                }

                if (e.Data.StartsWith("done"))
                {

                    Debug.WriteLine($"DONE: {activeFile.FilePath}");
                    this.activeFile.Progress = 100;
                    this.activeFile.UpdateTags();
                    this.worker.ReportProgress(100, this.activeFile);
                }
            }
        }

        private void GainProcess_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            //Debug.Write(e.Data);
        }

        private void ProcessGainOutput(string output)
        {
            this.GainLines = output.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        private void RunAnalysis(string executable)
        {
            var parameters = $"\"{Path.Combine(FolderPath, "*.mp3")}\"";
            var analysisStart = new ProcessStartInfo(executable, parameters);
            analysisStart.UseShellExecute = false;
            analysisStart.RedirectStandardOutput = true;
            analysisStart.RedirectStandardError = true;
            analysisStart.CreateNoWindow = true;

            var analysisProcess = new Process();

            analysisProcess.StartInfo = analysisStart;

            this.sortOutput = new StringBuilder();
            this.sortError = new StringBuilder();

            analysisProcess.OutputDataReceived += AnalysisProcess_OutputDataReceived;
            analysisProcess.ErrorDataReceived += AnalysisProcess_ErrorDataReceived;

            this.sortedFiles = this.Files.Select(x => x.Value.FilePath).ToList();
            sortedFiles.Sort();

            this.activeFile = this.Files[sortedFiles.First()];

            analysisProcess.Start();

            analysisProcess.BeginOutputReadLine();
            analysisProcess.BeginErrorReadLine();

            Debug.WriteLine($"STARTED ANALYSIS FOR {this.FolderName}");

            analysisProcess.WaitForExit();

            analysisProcess.OutputDataReceived -= AnalysisProcess_OutputDataReceived;
            analysisProcess.ErrorDataReceived -= AnalysisProcess_ErrorDataReceived;

            Debug.WriteLine($"FNISHED ANALYSIS FOR {this.FolderName}");
        }

        private void AnalysisProcess_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!String.IsNullOrEmpty(e.Data))
            {
                // Add the text to the collected output.
                sortError.Append(e.Data);
                //Debug.WriteLine(e.Data);

                //Debug.WriteLine(e.Data);

                if (e.Data.Contains("%"))
                {
                    var items = e.Data.Split('%');
                    var percentItems = items[0].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    var index = Convert.ToInt32(percentItems[0].Split('/')[0].Substring(1)) - 1;
                    var percent = Convert.ToInt32(percentItems[1]);

                    if (this.activeFile.FilePath != this.sortedFiles[index])
                    {
                        this.activeFile = this.Files[this.sortedFiles[index]];
                    }

                    if (percent != this.activeFile.Progress && (DateTime.Now - lastWrite).TotalSeconds > .250)
                    {
                        this.activeFile.Progress = percent;
                        this.RaiseChangedFile(this.activeFile);
                        lastWrite = DateTime.Now;
                    }
                }
            }
        }

        private void AnalysisProcess_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!String.IsNullOrEmpty(e.Data))
            {
                //Debug.WriteLine(e.Data);

                // Add the text to the collected output.
                sortOutput.Append(e.Data);
                //Debug.WriteLine(e.Data);
                ProcessAnalysisOutputLine(e.Data, ref this.activeFile);
            }
        }

        private void ProcessAnalysisOutputLine(string line, ref MP3GainFile gainFile)
        {
            //start mp3 file
            if (ProcessFilePath(ref gainFile, line))
            {
                return;
            }

            if (ProcessFileGain(gainFile, line))
            {
                return;
            }

            if (ProcessAlbumGain(line))
            {
                return;
            }
        }

        private void ProcessAnalysisOutput(string output)
        {
            this.AnalysisLines = output.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            MP3GainFile gainFile = null;

            foreach (var line in this.AnalysisLines)
            {
                //start mp3 file
                if (ProcessFilePath(ref gainFile, line))
                {
                    continue;
                }

                if (ProcessFileGain(gainFile, line))
                {
                    continue;
                }

                if (ProcessAlbumGain(line))
                {
                    break;
                }
            }
        }

        private bool ProcessAlbumGain(string line)
        {
            bool found = false;

            if (line.StartsWith($"Recommended \"Album\" mp3 gain change for all files:"))
            {
                var items = line.Split(new char[] { ':' });

                var change = items[1].Trim();

                if (Int32.TryParse(change, out var gain))
                {
                    this.SuggestedGain = gain;
                    found = true;
                }
            }

            if (line.StartsWith($"Recommended \"Album\" dB change for all files:"))
            {
                var items = line.Split(new char[] { ':' });

                var change = items[1].Trim();

                if (Double.TryParse(change, out var offset))
                {
                    this.DBOffset = offset;
                    found = true;
                }
            }

            return found;
        }

        private bool ProcessFileGain(MP3GainFile gainFile, string line)
        {
            bool found = false;
            bool done = false;

            if (line.StartsWith($"Recommended \"Track\" mp3 gain change:"))
            {
                var items = line.Split(new char[] { ':' });

                var change = items[1].Trim();

                if (gainFile != null && Int32.TryParse(change, out var gain))
                {
                    gainFile.SuggestedGain = gain;
                    found = true;
                    done = true;
                }
            }

            if (line.StartsWith($"Recommended \"Track\" dB change:"))
            {
                var items = line.Split(new char[] { ':' });

                var change = items[1].Trim();

                if (gainFile != null && double.TryParse(change, out var offset))
                {
                    gainFile.DBOffset = offset;
                    found = true;
                }
            }

            if (done)
            {
                //Debug.WriteLine($"Finished {gainFile.FilePath}");
                gainFile.Progress = 100;
                //this.RaiseChangedFile(gainFile);
                this.worker.ReportProgress(100, gainFile);
            }

            return found;
        }

        private void MoveToNextFile()
        {
            var index = this.sortedFiles.IndexOf(this.activeFile.FilePath);

            index++;

            if (index < this.sortedFiles.Count)
            {
                this.activeFile = this.Files[this.sortedFiles[index]];
            }
        }

        private bool ProcessFilePath(ref MP3GainFile gainFile, string line)
        {
            bool found = false;

            if (File.Exists(line))
            {
                gainFile = this.Files[line];
                found = true;
            }

            return found;
        }

        private void RaiseFoundFile(MP3GainFile file)
        {
            if (FoundFile != null)
            {
                this.FoundFile.Invoke(this, file);
            }
        }

        private void RaiseChangedFile(MP3GainFile file)
        {
            if (ChangedFile != null)
            {
                //this.ChangedFile.Invoke(this, file);
                this.worker.ReportProgress(0, file);
            }
        }

        internal void UndoGain(string executable, BackgroundWorker worker)
        {
            this.worker = worker;
            this.ExecuteUndoGain(executable);
        }

        private void ExecuteUndoGain(string executable)
        {
            this.UndoGainExecution = new ExecuteMP3Gain(executable,
                                                        "/u",
                                                        this.Files,
                                                        this.FolderPath,
                                                        "UNDO GAIN",
                                                        this.worker,
                                                        "Undoing mp3gain changes",
                                                        "                                                   ");

            this.UndoGainExecution.Execute();
        }

        private void ExecuteApplyGain(string executable)
        {
            this.ApplyGainExecution = new ExecuteMP3Gain(executable,
                                                        $"/o /g {this.SuggestedGain}",
                                                        this.Files,
                                                        this.FolderPath,
                                                        "APPLY GAIN",
                                                        this.worker,
                                                        "Applying gain",
                                                        "done");

            this.ApplyGainExecution.Execute();
        }
    }
}