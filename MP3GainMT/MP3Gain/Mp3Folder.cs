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
using System.IO;
using System.Linq;

namespace MP3GainMT.MP3Gain
{
    /// <summary>
    /// Represents a folder containing MP3 files.
    /// </summary>
    public class Mp3Folder
    {
        private int suggestedGain;

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
        public int UndoSuggestedGain { get; set; }

        /// <summary>
        /// Gets the suggested gain for the folder.
        /// </summary>
        public int SuggestedGain
        {
            get
            {
                if (this.Files.Count == 0)
                {
                    return Convert.ToInt32(Math.Round((this.DBOffset - MP3GainRow.TargetDiffDB) / Mp3File.FiveLog10Two));
                }

                var first = this.Files.First().Value;

                if (first.HasTags)
                {
                    this.suggestedGain = first.SuggestedAlbumGain;
                }
                else
                {
                    this.suggestedGain = Convert.ToInt32(Math.Round((this.DBOffset - MP3GainRow.TargetDiffDB) / Mp3File.FiveLog10Two));
                }

                return this.suggestedGain;
            }
        }

        /// <summary>
        /// Searches the folder for MP3 files and adds them to the Files dictionary.
        /// </summary>
        public void SearchFolder()
        {
            this.FindFiles(this.FolderPath);
        }

        /// <summary>
        /// Finds MP3 files in the specified folder path and adds them to the Files dictionary.
        /// </summary>
        private void FindFiles(string folderPath)
        {
            Helpers.UndoFileRenamesFromTextFile(folderPath);

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
        /// Raises the ChangedFile event.
        /// </summary>
        public void RaiseChangedFile(Mp3File file)
        {
            if (ChangedFile != null)
            {
                this.ChangedFile.Invoke(this, file);
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