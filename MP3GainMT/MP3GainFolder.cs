using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace WinFormMP3Gain
{
    internal class MP3GainFolder
    {
        private StringBuilder sortOutput;
        private StringBuilder sortError;
        private MP3GainFile activeFile;
        private BackgroundWorker worker;
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
            this.RunApplyGain(executable);
        }

        internal void ProcessFiles(string executable, BackgroundWorker worker)
        {
            this.worker = worker;
            this.RunAnalysis(executable);
        }

        private void RunApplyGain(string executable)
        {
            var gainStart = new ProcessStartInfo(executable, $"/g {this.SuggestedGain} \"{Path.Combine(FolderPath, "*.mp3")}\"");
            gainStart.UseShellExecute = false;
            gainStart.RedirectStandardOutput = true;
            gainStart.RedirectStandardError = true;
            gainStart.CreateNoWindow = true;

            var gainProcess = new Process();

            gainProcess.StartInfo = gainStart;
            gainProcess.Start();

            Debug.WriteLine($"{this.FolderName} started applying gain of {this.SuggestedGain}!");

            var stream = gainProcess.StandardError;
            this.GainOutput = stream.ReadToEnd();

            ProcessGainOutput(this.GainOutput);

            gainProcess.WaitForExit();
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
                        //this.RaiseChangedFile(this.activeFile);
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
    }
}