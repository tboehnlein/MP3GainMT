using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFormMP3Gain
{
    internal class MP3GainFile
    {
        public double DBOffset { get; set; } = 0.0;
        public int SuggestedGain { get; set; } = 0;

        public string FilePath { get; set; } = string.Empty;

        public string FileName { get; set; } = string.Empty;

        public string Folder { get; set; } = string.Empty;

        public string FolderPath { get; set; } = string.Empty;
        public int Progress { get; internal set; } = 0;

        public MP3GainFile(string file)
        { 
            this.FilePath = file;
            this.FileName = Path.GetFileName(this.FilePath);
            this.FolderPath = Path.GetDirectoryName(this.FilePath);
            this.Folder = this.FolderPath.Split(new char[] { Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries).ToList().Last();
            this.Progress = 0;
        }

        
    }
}
