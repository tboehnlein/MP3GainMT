using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace MP3GainMT.MP3Gain
{
    public class ExecuteMp3RGainAsync : ExecuteMp3GainAsync
    {
        public ExecuteMp3RGainAsync(string executablePath, string parameters, Dictionary<string, Mp3File> files, string folderPath, string actionName, System.ComponentModel.BackgroundWorker worker, string startingText, string endingText) 
            : base(executablePath, parameters, files, folderPath, actionName, worker, startingText, endingText)
        {
        }

        public override void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                fullLog.AppendLine(e.Data);

                // mp3rgain outputs lines like:
                // `  ~ [DRY RUN] CONAF - 228 - Al Franken Returns.mp3 (would apply 5 steps)`
                // or when applying:
                // `  ~ CONAF - 228 - Al Franken Returns.mp3 (applied 5 steps)`
                
                var data = e.Data.Trim();
                if (data.StartsWith("~ ") || data.StartsWith("v "))
                {
                    var fileStart = data.Substring(2).Replace("[DRY RUN] ", "").TrimStart();
                    var parenIndex = fileStart.LastIndexOf(" (");
                    if (parenIndex > 0)
                    {
                        var fileString = fileStart.Substring(0, parenIndex).Trim();
                        var fullPath = Path.IsPathRooted(fileString) ? fileString : Path.Combine(this.FolderPath, fileString);
                        
                        if (this.sortedFiles.Contains(fullPath))
                        {
                            this.activeFile = this.Files[this.fileLookUp[fullPath]];
                            
                            // mp3rgain doesn't output percentage progress lines, so when we see this line
                            // it means the file is 100% complete.
                            this.activeFile.Progress = 100;
                            this.activeFile.FlagUpdateTags();
                            int overallProgress = GetOverallProgress(100);
                            this.Worker.ReportProgress(overallProgress, new FileProgress(this.activeFile, this.activeFile.Progress));
                            this.filesFinished++;
                        }
                    }
                }
                
                ExtractActiveFile(e);
            }
        }
    }
}
