using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace MP3GainMT.MP3Gain
{
    internal class ExecuteMp3RGainSync : ExecuteMp3GainSync
    {
        public ExecuteMp3RGainSync(string executablePath, string parameters, Dictionary<string, Mp3File> files, string folderPath, string actionName, string startingText, string endingText) 
            : base(executablePath, parameters, files, folderPath, actionName, startingText, endingText)
        {
        }

        public override void ExtractActiveFile(DataReceivedEventArgs e)
        {
            if (e.Data != null && e.Data.Contains("\t"))
            {
                var values = e.Data.Split('\t');
                var fileString = values[0].Trim();

                var fullPath = Path.IsPathRooted(fileString) ? fileString : Path.Combine(this.FolderPath, fileString);

                if (this.sortedFiles.Contains(fullPath))
                {
                    this.activeFile = this.Files[fullPath];

                    // mp3rgain TSV format for `-s c`:
                    // 0: Filename
                    // 1: Track Gain (or -)
                    // 2: Min/Max Global Gain (e.g. 075,234)
                    // 3: Track DB Gain (or -)
                    // 4: Peak Amplitude (normalized float, e.g. 1.105010)
                    
                    // We need to extract the peak from index 4
                    if (values.Length > 4 && Double.TryParse(values[4].Trim(), out double peakValue))
                    {
                        // Convert normalized float peak to 16-bit PCM integer peak used by the rest of the application
                        double minValue = peakValue * 32768.0;
                        
                        this.activeFile.MaxNoClipGainTrackRaw = minValue;
                        this.activeFile.MaxNoClipGainAlbumRaw = minValue; // album raw isn't separated in mp3rgain TSV
                        this.activeFile.MaxNoClipGainTrack = GetMinGain(minValue);
                        this.activeFile.MaxNoClipGainAlbum = GetMinGain(minValue);
                    }
                }
            }
        }
    }
}
