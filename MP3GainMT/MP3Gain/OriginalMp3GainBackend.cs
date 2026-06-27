using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace MP3GainMT.MP3Gain
{
    public class OriginalMp3GainBackend : IMp3GainBackend
    {
        public virtual string Name => "Original mp3gain";
        public string ExecutablePath { get; set; }

        public OriginalMp3GainBackend(string executablePath)
        {
            this.ExecutablePath = executablePath;
        }

        public void ApplyGain(Mp3Folder folder, BackgroundWorker worker)
        {
            var applyGainExecution = new ExecuteMp3GainAsync(this.ExecutablePath,
                                                        $"/o /g {folder.SuggestedGain}",
                                                        folder.Files,
                                                        folder.FolderPath,
                                                        "APPLY GAIN",
                                                        worker,
                                                        "Applying gain",
                                                        "done");
            applyGainExecution.Execute();
        }

        public void UndoGain(Mp3Folder folder, BackgroundWorker worker)
        {
            int undoSuggestedGain = 0;
            var firstFile = folder.Files.Values.FirstOrDefault();
            if (firstFile != null)
            {
                undoSuggestedGain = -Convert.ToInt32(firstFile.GainUndoAlbum);
            }

            var undoGainExecution = new ExecuteMp3GainAsync(this.ExecutablePath,
                                                        "/u",
                                                        folder.Files,
                                                        folder.FolderPath,
                                                        "UNDO GAIN",
                                                        worker,
                                                        "Undoing mp3gain changes",
                                                        "                                                   ");
            undoGainExecution.Execute();
        }

        public void CalculateMaxGain(Mp3Folder folder, BackgroundWorker worker)
        {
            var execute = new ExecuteMp3GainSync(this.ExecutablePath,
                                             $"/o /s c",
                                             folder.Files,
                                             folder.FolderPath,
                                             "GET MAX GAIN",
                                             "Calculating Max Gain",
                                             "done");
            execute.Execute();
        }

        public void AnalyzeGain(Mp3Folder folder, BackgroundWorker worker)
        {
            new AnalysisRunner(this.ExecutablePath, folder, worker).Run();
        }

        private class AnalysisRunner
        {
            private string executablePath;
            private Mp3Folder folder;
            private BackgroundWorker worker;

            private Mp3File activeFile;
            private List<string> sortedFiles;
            private StringBuilder sortError;
            private StringBuilder sortOutput;
            private Dictionary<string, string> fileLookUp;
            private DateTime lastWrite = DateTime.Now;
            private int suggestedGain;

            public AnalysisRunner(string executablePath, Mp3Folder folder, BackgroundWorker worker)
            {
                this.executablePath = executablePath;
                this.folder = folder;
                this.worker = worker;
            }

            public void Run()
            {
                var parameters = $"\"{Path.Combine(folder.FolderPath, "*.mp3")}\"";
                var analysisStart = new ProcessStartInfo(executablePath, parameters);
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

                this.sortedFiles = folder.Files.Select(x => x.Value.FilePath).ToList();
                sortedFiles.Sort();

                this.fileLookUp = new Dictionary<string, string>();

                bool renameSuccess = Helpers.RandomlyRenameFiles(this.fileLookUp, this.sortedFiles);

                if (!renameSuccess)
                {
                    Helpers.UndoRandomlyRenameFiles(this.fileLookUp);
                    return;
                }

                this.sortedFiles = this.fileLookUp.Keys.ToList();
                if (sortedFiles.Any())
                {
                    this.activeFile = folder.Files[this.fileLookUp[sortedFiles.First()]];
                }

                analysisProcess.Start();
                analysisProcess.BeginOutputReadLine();
                analysisProcess.BeginErrorReadLine();
                analysisProcess.WaitForExit();

                analysisProcess.OutputDataReceived -= AnalysisProcess_OutputDataReceived;
                analysisProcess.ErrorDataReceived -= AnalysisProcess_ErrorDataReceived;

                Helpers.UndoRandomlyRenameFiles(this.fileLookUp);
                
                // Assign the final suggested gain back to folder if needed.
                // Mp3Folder already computes SuggestedGain from tags or DBOffset,
                // but analysis sets DBOffset and we should propagate it.
            }

            private void AnalysisProcess_ErrorDataReceived(object sender, DataReceivedEventArgs e)
            {
                if (!String.IsNullOrEmpty(e.Data))
                {
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

                        if (this.activeFile != null && this.activeFile.FilePath != folder.Files[this.fileLookUp[this.sortedFiles[index]]].FilePath)
                        {
                            this.activeFile = folder.Files[this.fileLookUp[this.sortedFiles[index]]];
                        }

                        if (this.activeFile != null && percent != this.activeFile.Progress && (DateTime.Now - lastWrite).TotalSeconds > .250)
                        {
                            this.activeFile.Progress = percent;
                            
                            // Emitting ChangedFile requires doing it through Mp3Folder if anyone is listening,
                            // but currently the worker progress is enough.
                            worker.ReportProgress(0, this.activeFile);
                            
                            lastWrite = DateTime.Now;

                            this.activeFile.FlagUpdateTags();
                            var fileOverall = (percent / 100.0) * (1.0 / this.sortedFiles.Count);
                            int overallProgress = Convert.ToInt32((fileOverall + ((float)index / this.sortedFiles.Count)) * 100.0);
                            this.worker.ReportProgress(overallProgress, new FileProgress(this.activeFile, this.activeFile.Progress));
                        }
                    }
                }
            }

            private void AnalysisProcess_OutputDataReceived(object sender, DataReceivedEventArgs e)
            {
                if (!String.IsNullOrEmpty(e.Data))
                {
                    sortOutput.Append(e.Data);
                    ProcessAnalysisOutputLine(e.Data);
                }
            }

            private void ProcessAnalysisOutputLine(string line)
            {
                if (ProcessFilePath(line)) return;
                if (ProcessFileGain(line)) return;
                if (ProcessAlbumGain(line)) return;
            }

            private bool ProcessFilePath(string line)
            {
                if (File.Exists(line) && this.fileLookUp.ContainsKey(line))
                {
                    this.activeFile = folder.Files[this.fileLookUp[line]];
                    return true;
                }
                return false;
            }

            private bool ProcessFileGain(string line)
            {
                bool found = false;
                bool done = false;

                if (line.StartsWith("Recommended \"Track\" mp3 gain change:"))
                {
                    var items = line.Split(new char[] { ':' });
                    var change = items[1].Trim();
                    if (this.activeFile != null && Int32.TryParse(change, out var gain))
                    {
                        this.activeFile.SuggestedGain = gain;
                        found = true;
                        done = true;
                    }
                }

                if (line.StartsWith("Recommended \"Track\" dB change:"))
                {
                    var items = line.Split(new char[] { ':' });
                    var change = items[1].Trim();
                    if (this.activeFile != null && double.TryParse(change, out var offset))
                    {
                        this.activeFile.DBOffset = offset;
                        found = true;
                    }
                }

                if (done && this.activeFile != null)
                {
                    this.activeFile.Progress = 100;
                    this.activeFile.FlagUpdateTags();
                    var fileOverall = 1.0 / this.sortedFiles.Count;
                    int overallProgress = Convert.ToInt32((fileOverall + ((float)(this.fileLookUp.Values.Cast<string>().ToList().IndexOf(this.activeFile.FilePath)) / this.sortedFiles.Count)) * 100.0);
                    this.worker.ReportProgress(overallProgress, new FileProgress(this.activeFile, this.activeFile.Progress));
                }

                return found;
            }

            private bool ProcessAlbumGain(string line)
            {
                bool found = false;

                if (line.StartsWith("Recommended \"Album\" mp3 gain change for all files:"))
                {
                    var items = line.Split(new char[] { ':' });
                    var change = items[1].Trim();
                    if (Int32.TryParse(change, out var gain))
                    {
                        // Usually Mp3Folder reads this from tags, but let's store it on the first file or pass it to folder
                        this.suggestedGain = gain;
                        // UndoSuggestedGain gets set here in Original Mp3Folder logic
                        found = true;
                    }
                }

                if (line.StartsWith("Recommended \"Album\" dB change for all files:"))
                {
                    var items = line.Split(new char[] { ':' });
                    var change = items[1].Trim();
                    if (Double.TryParse(change, out var offset))
                    {
                        folder.DBOffset = offset;
                        found = true;
                    }
                }

                return found;
            }
        }
    }
}
