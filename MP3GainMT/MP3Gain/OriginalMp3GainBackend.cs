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
        public virtual BackendSchedulingStrategy SchedulingStrategy => BackendSchedulingStrategy.StandardConcurrent;

        protected virtual string ArgFormat => "/o ";
        protected virtual string ArgApply => "/g ";
        protected virtual string ArgCheck => "/s c ";
        protected virtual string ArgUndo => "/u ";

        public OriginalMp3GainBackend(string executablePath)
        {
            this.ExecutablePath = executablePath;
        }

        protected virtual string GetExtraArgs(int threadCount)
        {
            return string.Empty;
        }

        public virtual void ApplyGain(Mp3Folder folder, BackgroundWorker worker, int threadCount = 1)
        {
            var applyGainExecution = new ExecuteMp3GainAsync(this.ExecutablePath,
                                                        $"{ArgFormat}{ArgApply}{folder.SuggestedGain} {GetExtraArgs(threadCount)}".Trim(),
                                                        folder.Files,
                                                        folder.FolderPath,
                                                        "APPLY GAIN",
                                                        worker,
                                                        "Applying Track Gain",
                                                        "done");
            applyGainExecution.Execute();
        }

        public virtual void UndoGain(Mp3Folder folder, BackgroundWorker worker, int threadCount = 1)
        {
            int undoSuggestedGain = -folder.SuggestedGain;
            if (folder.Files.Values.Any())
            {
                var firstFile = folder.Files.Values.First();
                undoSuggestedGain = -Convert.ToInt32(firstFile.GainUndoAlbum);
            }

            var undoGainExecution = new ExecuteMp3GainAsync(this.ExecutablePath,
                                                        $"{ArgUndo}{GetExtraArgs(threadCount)}".Trim(),
                                                        folder.Files,
                                                        folder.FolderPath,
                                                        "UNDO GAIN",
                                                        worker,
                                                        "Undoing Track Gain",
                                                        "done");
            undoGainExecution.Execute();
        }

        public virtual void CalculateMaxGain(Mp3Folder folder, BackgroundWorker worker, int threadCount = 1)
        {
            var execute = new ExecuteMp3GainSync(this.ExecutablePath,
                                             $"{ArgFormat}{ArgCheck}{GetExtraArgs(threadCount)}".Trim(),
                                             folder.Files,
                                             folder.FolderPath,
                                             "GET MAX GAIN",
                                             "Calculating Max Gain",
                                             "done");
            execute.Execute();
        }

        public virtual void AnalyzeGain(Mp3Folder folder, BackgroundWorker worker, int threadCount = 1)
        {
            new AnalysisRunner(this.ExecutablePath, folder, worker, GetExtraArgs(threadCount)).Run();
        }

        protected class AnalysisRunner
        {
            private string executablePath;
            private Mp3Folder folder;
            private BackgroundWorker worker;
            private string extraArgs;

            private Mp3File activeFile;
            private List<string> sortedFiles;
            private StringBuilder sortError;
            private StringBuilder sortOutput;
            private Dictionary<string, string> fileLookUp;
            private DateTime lastWrite = DateTime.Now;
            private int suggestedGain;

            public AnalysisRunner(string executablePath, Mp3Folder folder, BackgroundWorker worker, string extraArgs = "")
            {
                this.executablePath = executablePath;
                this.folder = folder;
                this.worker = worker;
                this.extraArgs = extraArgs;
            }

            public void Run()
            {
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

                var parameters = $"{extraArgs} *.mp3".Trim();
                var analysisStart = new ProcessStartInfo(executablePath, parameters);
                analysisStart.WorkingDirectory = folder.FolderPath;
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
                var fullPath = Path.IsPathRooted(line) ? line : Path.Combine(folder.FolderPath, line);
                if (File.Exists(fullPath) && this.fileLookUp.ContainsKey(fullPath))
                {
                    this.activeFile = folder.Files[this.fileLookUp[fullPath]];
                    return true;
                }
                return false;
            }

            private bool ProcessFileGain(string line)
            {
                bool found = false;
                bool done = false;

                string trimmedLine = line.TrimStart();

                if (trimmedLine.StartsWith("Recommended \"Track\" mp3 gain change:"))
                {
                    var items = trimmedLine.Split(new char[] { ':' });
                    var change = items[1].Trim();
                    if (this.activeFile != null && Int32.TryParse(change, out var gain))
                    {
                        this.activeFile.SuggestedGain = gain;
                        found = true;
                        done = true;
                    }
                }

                if (trimmedLine.StartsWith("Recommended \"Track\" dB change:"))
                {
                    var items = trimmedLine.Split(new char[] { ':' });
                    var change = items[1].Trim();
                    if (this.activeFile != null && double.TryParse(change, out var offset))
                    {
                        this.activeFile.DBOffset = offset;
                        found = true;
                    }
                }

                if (trimmedLine.StartsWith("Max PCM sample at current gain:"))
                {
                    var items = trimmedLine.Split(new char[] { ':' });
                    var change = items[1].Trim();
                    if (this.activeFile != null && double.TryParse(change, out var maxPcm))
                    {
                        this.activeFile.MaxNoClipGainTrackRaw = maxPcm;
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
