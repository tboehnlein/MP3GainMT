using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using TagLib;

namespace WinFormMP3Gain
{
    internal class MP3GainFile
    {
        public double DBOffset { get; set; } = 0.0;
        public int SuggestedGain { get; set; } = 0;

        public string FilePath { get; set; } = string.Empty;
        public string Artist { get; private set; } = string.Empty;
        public string Album { get; private set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;

        public string Folder { get; set; } = string.Empty;

        public string FolderPath { get; set; } = string.Empty;
        public int Progress { get; internal set; } = 0;

        public bool HasTags { get; internal set; } = false;

        public bool HasErrors => this.ErrorMessages.Count > 0;

        public List<string> ErrorMessages { get; private set; } = new List<string>();
        public bool Updated { get; internal set; }

        public MP3GainFile(string file)
        {
            this.FilePath = file;
            this.Progress = 0;
            this.FileName = Path.GetFileName(this.FilePath);
            this.FolderPath = Path.GetDirectoryName(this.FilePath);

            try
            {
                var folders = this.FolderPath.Split(new char[] { Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries).ToList();

                var first = folders[2];
                var secondLast = $"{folders[folders.Count - 2]}\\";
                var last = folders.Last();
                var displayFolder = $"{Path.GetPathRoot(this.FilePath)}{first}\\ ... \\{secondLast}{last}";

                this.Folder = displayFolder;
            }
            catch (Exception ex)
            {
                GenerateErrorMessage("Display Folder Creation Error", ex);
            }
            
        }

        private void GenerateErrorMessage(string header, Exception ex)
        {
            var message = $"{header}: {ex.Message}";            
            this.ErrorMessages.Add(message);

            Debug.WriteLine($"{message} ({this.FilePath})");
        }

        internal void ExtractTags()
        {
            if (!this.HasTags)
            {
                try
                {
                    this.HasTags = true;
                    var tagFile = TagLib.File.Create(this.FilePath);
                    if (tagFile.Tag.Performers.Length > 0) { this.Artist = tagFile.Tag.Performers[0]; }
                    this.Album = tagFile.Tag.Album;
                    this.Updated = true;
                }
                catch (CorruptFileException ex)
                {
                    GenerateErrorMessage("Tag Extract Error", ex);
                }
            }
            
        }
    }
}