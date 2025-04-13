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
using System.Reflection;
using System.Text;
using System.Threading;

namespace MP3GainMT.MP3Gain
{


    /// <summary>
    /// Represents a folder containing MP3 files and provides methods to analyze and apply gain adjustments.
    /// </summary>
    public class Mp3Folder
    {
        private static DateTime lastWrite = DateTime.Now;
        private Mp3File activeFile;
        private List<string> sortedFiles;
        private StringBuilder sortError;
        private StringBuilder sortOutput;
        private BackgroundWorker worker;
        private int suggestedGain;
        private Dictionary<string, string> fileLookUp;

        /// <summary>
        /// Initializes a new instance of the Mp3Folder class with the specified folder path.
        /// </summary>
        /// <param name="path">The path to the folder containing MP3 files.</param>
        public Mp3Folder(string path)
        {
            this.FolderPath = path;
            this.FolderName = Path.GetFileName(path);
        }

        /// <summary>
        /// Event triggered when an MP3 file is changed.
        /// </summary>
        public event EventHandler<Mp3File> ChangedFile;

        /// <summary>
        /// Event triggered when an MP3 file is found.
        /// </summary>
        public event EventHandler<Mp3File> FoundFile;

        /// <summary>
        /// Gets the execution context for applying gain adjustments.
        /// </summary>
        public ExecuteMp3GainAsync ApplyGainExecution { get; private set; }

        /// <summary>
        /// Gets or sets the dB offset for the folder.
        /// </summary>
        public double DBOffset { get; set; } = 0.0;

        /// <summary>
        /// Gets or sets the dictionary of MP3 files in the folder.
        /// </summary>
        public Dictionary<string, Mp3File> Files { get; set; } = new Dictionary<string, Mp3File>();

        /// <summary>
        /// Gets or sets the name of the folder.
        /// </summary>
        public string FolderName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the path to the folder.
        /// </summary>
        public string FolderPath { get; set; } = string.Empty;

        /// <summary>
        /// Gets the list of MP3 file paths in the folder.
        /// </summary>
        public List<string> MP3Files
        {
            get
            {
                return Files.Select(x => x.Value.FilePath).ToList();
            }
        }

        /// <summary>
        /// Gets the total length of all MP3 files in the folder.
        /// </summary>
        public long Length => this.Files.Sum(x => x.Value.Length);

        /// <summary>
        /// Gets the suggested gain for undoing the last gain adjustment.
        /// </summary>
        public int UndoSuggestedGain { get; private set; }

        /// <summary>
        /// Gets the suggested gain for the folder.
        /// </summary>
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

        /// <summary>
        /// Gets the execution context for undoing gain adjustments.
        /// </summary>
        public ExecuteMp3GainAsync UndoGainExecution { get; private set; }

        /// <summary>
        /// Searches the folder for MP3 files and adds them to the Files dictionary.
        /// </summary>
        public void SearchFolder()
        {
            this.FindFiles(this.FolderPath);
        }

        /// <summary>
        /// Applies gain adjustments to the MP3 files in the folder.
        /// </summary>
        /// <param name="executable">The path to the executable for applying gain.</param>
        /// <param name="worker">The background worker for reporting progress.</param>
        internal void ApplyGainFolder(string executable, BackgroundWorker worker)
        {
            this.worker = worker;
            this.ExecuteApplyGain(executable);
        }

        /// <summary>
        /// Analyzes the gain of the MP3 files in the folder.
        /// </summary>
        /// <param name="executable">The path to the executable for analyzing gain.</param>
        /// <param name="worker">The background worker for reporting progress.</param>
        internal void AnalyzeGainFolder(string executable, BackgroundWorker worker)
        {
            this.worker = worker;
            this.ExecuteAnalyzeGain(executable);
        }

        /// <summary>
        /// Undoes the gain adjustments for the MP3 files in the folder.
        /// </summary>
        /// <param name="executable">The path to the executable for undoing gain.</param>
        /// <param name="worker">The background worker for reporting progress.</param>
        internal void UndoGain(string executable, BackgroundWorker worker)
        {
            this.worker = worker;
            this.ExecuteUndoGain(executable);
        }

        /// <summary>
        /// Handles the error data received from the analysis process.
        /// </summary>
        private void AnalysisProcess_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!String.IsNullOrEmpty(e.Data))
            {
                // Add the text to the collected output.
                sortError.Append(e.Data);

                if (e.Data.Contains("%"))
                {
                    var items = e.Data.Split('%');
                    var percentItems = items[0].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    var index = 0;
                    var percent = 0;

                    if (percentItems[0].Contains('/'))
                    {
                        index = Convert.ToInt32(percentItems[0].Split('/')[0].Substring(1)) - 1;
                        percent = Convert.ToInt32(percentItems[1]);
                    }
                    else
                    {
                        percent = Convert.ToInt32(percentItems[0]);
                    }

                    if (this.activeFile.FilePath != this.Files[this.fileLookUp[this.sortedFiles[index]]].FilePath)
                    {
                        this.activeFile = this.Files[this.fileLookUp[this.sortedFiles[index]]];
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
                    }
                }
            }
        }

        /// <summary>
        /// Handles the output data received from the analysis process.
        /// </summary>
        private void AnalysisProcess_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!String.IsNullOrEmpty(e.Data))
            {
                // Add the text to the collected output.
                sortOutput.Append(e.Data);
                ProcessAnalysisOutputLine(e.Data, ref this.activeFile);
            }
        }

        /// <summary>
        /// Executes the apply gain process.
        /// </summary>
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
        }

        /// <summary>
        /// Resets the suggested gain to zero.
        /// </summary>
        private void ResetSuggestedGain()
        {
            if (this.SuggestedGain != 0)
            {
                this.UndoSuggestedGain = this.suggestedGain;
            }

            this.suggestedGain = 0;
        }

        /// <summary>
        /// Executes the undo gain process.
        /// </summary>
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

        /// <summary>
        /// Restores the suggested gain to the previous value.
        /// </summary>
        private void RestoreUndoSuggestedGain()
        {
            this.suggestedGain = this.UndoSuggestedGain;
        }

        /// <summary>
        /// Records the undo suggested tag.
        /// </summary>
        private void RecordUndoSuggestedTag()
        {
            var undoAlbumGain = -Convert.ToInt32(this.Files.Values.First().GainUndoAlbum);

            if (undoAlbumGain != 0)
            {
                this.UndoSuggestedGain = undoAlbumGain;
            }
        }

        /// <summary>
        /// Finds MP3 files in the specified folder path and adds them to the Files dictionary.
        /// </summary>
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

        /// <summary>
        /// Processes the album gain from the analysis output line.
        /// </summary>
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

        /// <summary>
        /// Processes the analysis output line and updates the gain file accordingly.
        /// </summary>
        private void ProcessAnalysisOutputLine(string line, ref Mp3File gainFile)
        {
            // Start MP3 file
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

        /// <summary>
        /// Processes the file gain from the analysis output line.
        /// </summary>
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
                gainFile.Progress = 100;
                gainFile.FlagUpdateTags();
                var fileOverall = 1.0 / this.sortedFiles.Count;
                int overallProgress = Convert.ToInt32((fileOverall + ((float)(this.fileLookUp.Values.Cast<string>().ToList().IndexOf(gainFile.FilePath)) / this.sortedFiles.Count)) * 100.0);
                this.worker.ReportProgress(overallProgress, new FileProgress(gainFile, gainFile.Progress));
            }

            return found;
        }

        /// <summary>
        /// Processes the file path from the analysis output line.
        /// </summary>
        private bool ProcessFilePath(ref Mp3File gainFile, string line)
        {
            bool found = false;

            if (File.Exists(line))
            {
                gainFile = this.Files[this.fileLookUp[line]];
                found = true;
            }

            return found;
        }

        /// <summary>
        /// Raises the ChangedFile event.
        /// </summary>
        private void RaiseChangedFile(Mp3File file)
        {
            if (ChangedFile != null)
            {
                this.worker.ReportProgress(0, file);
            }
        }

        /// <summary>
        /// Raises the FoundFile event.
        /// </summary>
        private void RaiseFoundFile(Mp3File file)
        {
            if (FoundFile != null)
            {
                this.FoundFile.Invoke(this, file);
            }
        }

        /// <summary>
        /// Executes the analyze gain process.
        /// </summary>
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

            this.fileLookUp = new Dictionary<string, string>();

            bool renameSuccess = Helpers.RandomlyRenameFiles(this.fileLookUp, this.sortedFiles);

            if (!renameSuccess)
            {
                Helpers.UndoRandomlyRenameFiles(this.fileLookUp);

                return;
            }

            this.sortedFiles = this.fileLookUp.Keys.ToList();

            this.activeFile = this.Files[this.fileLookUp[sortedFiles.First()]];

            analysisProcess.Start();

            analysisProcess.BeginOutputReadLine();
            analysisProcess.BeginErrorReadLine();

            analysisProcess.WaitForExit();

            analysisProcess.OutputDataReceived -= AnalysisProcess_OutputDataReceived;
            analysisProcess.ErrorDataReceived -= AnalysisProcess_ErrorDataReceived;

            Helpers.UndoRandomlyRenameFiles(this.fileLookUp);
        }

        /// <summary>
        /// Sets the alternative color flag for all MP3 files in the folder.
        /// </summary>
        internal void SetAltColorFlag(bool useAlternativeColor)
        {
            foreach (var file in this.Files.Values)
            {
                file.UseAlternativeColor = useAlternativeColor;
            }
        }
    }
}