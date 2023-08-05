﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MP3GainMT.MP3Gain
{
    public class Mp3Folder
    {
        private static DateTime lastWrite = DateTime.Now;
        private Mp3File activeFile;
        private List<string> sortedFiles;
        private StringBuilder sortError;
        private StringBuilder sortOutput;
        private BackgroundWorker worker;
        private int suggestedGain;

        public Mp3Folder(string path)
        {
            this.FolderPath = path;
            this.FolderName = Path.GetFileName(path);
        }

        public event EventHandler<Mp3File> ChangedFile;

        public event EventHandler<Mp3File> FoundFile;

        public ExecuteMp3GainAsync ApplyGainExecution { get; private set; }
        public double DBOffset { get; set; } = 0.0;
        public Dictionary<string, Mp3File> Files { get; set; } = new Dictionary<string, Mp3File>();
        public string FolderName { get; set; } = string.Empty;
        public string FolderPath { get; set; } = string.Empty;

        public List<string> MP3Files
        {
            get
            {
                return Files.Select(x => x.Value.FilePath).ToList();
            }
        }

        public long Length => this.Files.Sum(x => x.Value.Length);

        public int UndoSuggestedGain { get; private set; }
        public int SuggestedGain
        {
            get
            {
                if (this.Files.Count == 0)
                {
                    return this.suggestedGain;
                }

                var first = this.Files.First().Value;

                if (first.HasTags)
                {
                    this.suggestedGain = first.SuggestedAlbumGain;
                }
                
                return this.suggestedGain;
            }
        }

        public ExecuteMp3GainAsync UndoGainExecution { get; private set; }

        public void SearchFolder()
        {
            this.FindFiles(this.FolderPath);
        }

        internal void ApplyGainFolder(string executable, BackgroundWorker worker)
        {
            this.worker = worker;
            this.ExecuteApplyGain(executable);
        }

        internal void AnalyzeGainFolder(string executable, BackgroundWorker worker)
        {
            this.worker = worker;
            this.ExecuteAnalyzeGain(executable);
        }

        internal void UndoGain(string executable, BackgroundWorker worker)
        {
            this.worker = worker;
            this.ExecuteUndoGain(executable);
        }

        private void AnalysisProcess_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!String.IsNullOrEmpty(e.Data))
            {
                // Add the text to the collected output.
                sortError.Append(e.Data);
                //Debug.WriteLine(e.Data);

                //Debug.WriteLine(e.Data);

                //Debug.Write(e.Data);

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

                        this.activeFile.FlagUpdateTags();
                        var fileOverall = (percent / 100.0) * (1.0 / this.sortedFiles.Count);
                        int overallProgress = Convert.ToInt32((fileOverall + ((float)index / this.sortedFiles.Count)) * 100.0);
                        this.worker.ReportProgress(overallProgress, new FileProgress(this.activeFile, this.activeFile.Progress));

                        Debug.WriteLine($"OVERALL: {overallProgress} PROGRESS: {fileOverall} for {this.activeFile.FileName}");
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
                //Debug.Write(e.Data);
                ProcessAnalysisOutputLine(e.Data, ref this.activeFile);
            }
        }

        private void ExecuteApplyGain(string executable)
        {
            this.ApplyGainExecution = new ExecuteMp3GainAsync(executable,
                                                        $"/o /g {this.SuggestedGain}",
                                                        this.Files,
                                                        this.FolderPath,
                                                        "APPLY GAIN",
                                                        this.worker,
                                                        "Applying gain",
                                                        "done");

            this.ApplyGainExecution.Execute();

            ResetSuggestedGain();

            //Debug.WriteLine($"APPLY SUGG: {this.SuggestedGain} PREV: {this.UndoSuggestedGain} for {this.FolderName}");
        }

        private void ResetSuggestedGain()
        {
            if (this.SuggestedGain != 0)
            {
                this.UndoSuggestedGain = this.suggestedGain;
                //Debug.WriteLine($"APPLY STORED PREV {this.UndoSuggestedGain} for {this.FolderName}");
            }

            this.suggestedGain = 0;
        }

        private void ExecuteUndoGain(string executable)
        {
            RecordUndoSuggestedTag();

            this.UndoGainExecution = new ExecuteMp3GainAsync(executable,
                                                        "/u",
                                                        this.Files,
                                                        this.FolderPath,
                                                        "UNDO GAIN",
                                                        this.worker,
                                                        "Undoing mp3gain changes",
                                                        "                                                   ");

            this.UndoGainExecution.Execute();

            RestoreUndoSuggestedGain();
        }

        private void RestoreUndoSuggestedGain()
        {
            this.suggestedGain = this.UndoSuggestedGain;

            //Debug.WriteLine($"UNDO SUGG: {this.SuggestedGain} PREV: {this.UndoSuggestedGain} for {this.FolderName}");
        }

        private void RecordUndoSuggestedTag()
        {
            var undoAlbumGain = -Convert.ToInt32(this.Files.Values.First().GainUndoAlbum);

            if (undoAlbumGain != 0)
            {
                this.UndoSuggestedGain = undoAlbumGain;
                //Debug.WriteLine($"UNDO RECOVERED PREV: {this.UndoSuggestedGain} for {this.FolderName}");
            }
        }

        private void FindFiles(string folderPath)
        {
            var files = Directory.GetFiles(folderPath, "*.mp3", SearchOption.TopDirectoryOnly);

            foreach (var file in files)
            {
                if (File.Exists(file))
                {
                    var mp3File = new Mp3File(file);

                    this.Files.Add(mp3File.FilePath, mp3File);

                    this.RaiseFoundFile(mp3File);
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
                    this.suggestedGain = gain;
                    this.UndoSuggestedGain = gain;

                    Debug.WriteLine($"SUGGESTED ALBUM GAIN: {this.SuggestedGain} PREV: {this.UndoSuggestedGain} for {this.FolderName}");

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

                Debug.WriteLine($"SUGGESTED ALBUM DB OFFSET: {this.DBOffset} for {this.FolderName}");
            }

            return found;
        }

        private void ProcessAnalysisOutputLine(string line, ref Mp3File gainFile)
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

        private bool ProcessFileGain(Mp3File gainFile, string line)
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
                //this.worker.ReportProgress(100, gainFile);
                gainFile.FlagUpdateTags();
                var fileOverall = 1.0 / this.sortedFiles.Count;
                int overallProgress = Convert.ToInt32((fileOverall + ((float)(this.sortedFiles.IndexOf(gainFile.FileName)) / this.sortedFiles.Count)) * 100.0);
                this.worker.ReportProgress(overallProgress, new FileProgress(gainFile, gainFile.Progress));
            }

            return found;
        }

        private bool ProcessFilePath(ref Mp3File gainFile, string line)
        {
            bool found = false;

            if (File.Exists(line))
            {
                gainFile = this.Files[line];
                found = true;
            }

            return found;
        }

        private void RaiseChangedFile(Mp3File file)
        {
            if (ChangedFile != null)
            {
                //this.ChangedFile.Invoke(this, file);
                this.worker.ReportProgress(0, file);
            }
        }

        private void RaiseFoundFile(Mp3File file)
        {
            if (FoundFile != null)
            {
                this.FoundFile.Invoke(this, file);
            }
        }

        private void ExecuteAnalyzeGain(string executable)
        {
            //this.ApplyGainExecution = new ExecuteMp3GainAsync(executable,
            //                                            $"\"{Path.Combine(FolderPath, "*.mp3")}\"",
            //                                            this.Files,
            //                                            this.FolderPath,
            //                                            "ANALYZE GAIN",
            //                                            this.worker,
            //                                            "Applying gain",
            //                                            "done");

            //this.ApplyGainExecution.Execute();

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

            //Debug.WriteLine($"STARTED ANALYSIS FOR {this.FolderName}");

            analysisProcess.WaitForExit();

            analysisProcess.OutputDataReceived -= AnalysisProcess_OutputDataReceived;
            analysisProcess.ErrorDataReceived -= AnalysisProcess_ErrorDataReceived;

            //Debug.WriteLine($"FINSHED ANALYSIS FOR {this.FolderName}");
        }

        internal void SetAltColorFlag(bool useAlternativeColor)
        {
            foreach (var file in this.Files.Values)
            {
                file.UseAlternativeColor = useAlternativeColor;
            }
        }
    }
}