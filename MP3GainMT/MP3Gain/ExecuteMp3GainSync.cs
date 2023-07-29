using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace MP3GainMT.MP3Gain
{
    internal class ExecuteMp3GainSync : ExecuteMp3GainAsync
    {
        public ExecuteMp3GainSync(string executable,
                                  string arguments,
                                  Dictionary<string, Mp3File> files,
                                  string folder,
                                  string actionName,
                                  string fileOutputPrefix,
                                  string endingOutputText) : base(executable,
                                                                  arguments,
                                                                  files,
                                                                  folder,
                                                                  actionName,
                                                                  null,
                                                                  fileOutputPrefix,
                                                                  endingOutputText)
        {
        }

        public override void Execute()
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

        public override void ExtractActiveFile(DataReceivedEventArgs e)
        {
            if (e.Data != null && !e.Data.StartsWith("File"))
            {
                var values = e.Data.Split('\t');

                var fileString = values[0].Trim();

                if (this.sortedFiles.Contains(fileString))
                {
                    this.activeFile = this.Files[fileString];

                    if(!Double.TryParse(values[3].Trim(), out double minValue))
                    {
                        minValue = 0;
                    }

                    if (!Double.TryParse(values[8].Trim(), out double minAlbumValue))
                    {
                        minAlbumValue = 0;
                    }

                    this.activeFile.MaxNoClipGainTrackRaw = minValue;
                    this.activeFile.MaxNoClipGainAlbumRaw = minAlbumValue;
                    this.activeFile.MaxNoClipGainTrack = GetMinGain(minValue);
                    this.activeFile.MaxNoClipGainAlbum = GetMinGain(minAlbumValue);
                }
            }
        }

        private double GetMinGain(double minValue)
        {
            // convert to dB
            var final = Math.Log10(32767.0 / minValue) / Math.Log10(2.0) * 6.0206;

            // round to nearest dB
            return final;// Mp3File.DbFlooring(final);
        }

        public override void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
        }

        public override void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            ExtractActiveFile(e);
        }

        public override void ProcessFileEnding(DataReceivedEventArgs e)
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
                    this.activeFile.FlagUpdateTags();
                    int overallProgress = GetOverallProgress(100);
                    this.Worker.ReportProgress(overallProgress, new FileProgress(this.activeFile, this.activeFile.Progress));
                    filesFinished++;
                }
            }
        }

        public override void UpdateProgress(DataReceivedEventArgs e)
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
    }
}