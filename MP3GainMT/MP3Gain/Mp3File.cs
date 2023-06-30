using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using TagLib;

namespace MP3GainMT.MP3Gain
{
    public class Mp3File
    {
        public const string TagMp3GainAlbumMinMax = "MP3GAIN_ALBUM_MINMAX";
        public const string TagMp3GainMinMax = "MP3GAIN_MINMAX";
        public const string TagMp3GainUndo = "MP3GAIN_UNDO";
        public const string TagReplayAlbumGain = "REPLAYGAIN_ALBUM_GAIN";
        public const string TagReplayAlbumPeak = "REPLAYGAIN_ALBUM_PEAK";
        public const string TagReplayTrackGain = "REPLAYGAIN_TRACK_GAIN";
        public const string TagReplayTrackPeak = "REPLAYGAIN_TRACK_PEAK";
        public double GainAlbumMax = 0.0;
        public double GainAlbumMin = 0.0;
        public double GainMax = 0.0;
        public double GainMin = 0.0;
        public double GainUndoAlbum = 0.0;
        public string GainUndoLabel = string.Empty;
        public double GainUndoTrack = 0.0;
        public double ReplayAlbumGain = 0.0;
        public double ReplayAlbumPeak = 0.0;
        public double ReplayTrackGain = 0.0;
        public double ReplayTrackPeak = 0.0;
        private const double divide = 2.0 / 3.0;

        public Mp3File(string file)
        {
            this.FilePath = file;
            this.Progress = 0;
            this.FileName = Path.GetFileName(this.FilePath);
            this.FolderPath = Path.GetDirectoryName(this.FilePath);

            try
            {
                var folders = this.FolderPath.Split(new char[] { Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries).ToList();

                if (folders.Count <= 4)
                {
                    this.Folder = this.FolderPath;

                    return;
                }

                var first = folders[1];
                
                var fourthLast = GetRightSubFolder(folders, 4);
                var thirdLast = GetRightSubFolder(folders, 3);
                var secondLast = GetRightSubFolder(folders, 2);
                var last = GetRightSubFolder(folders, 1);
                var split = $"\\ ... \\";

                var displayFolder = $"{Path.GetPathRoot(this.FilePath)}{first}{split}{fourthLast}{thirdLast}{secondLast}{last}";

                this.Folder = displayFolder;
            }
            catch (Exception ex)
            {
                GenerateErrorMessage("Display Folder Creation Error", ex);
            }
        }

        private static string GetRightSubFolder(List<string> folders, int index)
        {
            var folder = string.Empty;

            if (folders.Count > (index + 1))
            {
                folder = $"{folders[folders.Count - index]}\\";
            }

            return folder;
        }

        public string Album { get; private set; } = string.Empty;
        public string AlbumArtist { get; private set; }
        public string Artist { get; private set; } = string.Empty;
        public double DBOffset { get; set; } = 0.0;
        public List<string> ErrorMessages { get; private set; } = new List<string>();
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string Folder { get; set; } = string.Empty;
        public string FolderPath { get; set; } = string.Empty;
        public bool HasErrors => this.ErrorMessages.Count > 0;
        public bool HasTags { get; internal set; } = false;
        public int Progress { get; internal set; } = 0;

        public double ReplayAlbumGainRounded
        {
            get
            {
                var gain = Math.Round(ReplayAlbumGain * divide) / divide;

                return gain;
            }
        }

        public double ReplayTrackGainRounded
        {
            get
            {
                var gain = Math.Round(ReplayTrackGain * divide) / divide;

                return gain;
            }
        }

        public int SourceIndex { get; internal set; }
        public double SuggestedGain { get; set; } = 0;
        public string Title { get; set; } = string.Empty;
        public uint Track { get; private set; }
        public bool Updated { get; internal set; }
        public long Length { get; private set; }

        public bool IsFileLocked()
        {
            try
            {
                FileInfo file = new FileInfo(this.FilePath);

                using (FileStream stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    stream.Close();
                }
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }

            //file is not locked
            return false;
        }

        public void WriteDebug()
        {
            Debug.WriteLine(this.FileName);
            Debug.WriteLine($"[{this.AlbumArtist} - {this.Album}] \\ {this.Track} - {this.Artist} - {this.Title}");
            Debug.WriteLine($"MIN: {this.GainMin} MAX: {this.GainMax}");
            Debug.WriteLine($"TRACK UNDO: {this.GainUndoTrack} ALBUM UNDO: {this.GainUndoAlbum} LABEL UNDO: {this.GainUndoLabel}");
            Debug.WriteLine($"ALBUM GAIN: {this.ReplayAlbumGain} ALBUM PEAK: {this.ReplayAlbumPeak}");
            Debug.WriteLine($"TRACK GAIN: {this.ReplayTrackGain} TRACK PEAK: {this.ReplayTrackPeak}");
            Debug.WriteLine($"ALBUM GAIN ROUNDED: {this.ReplayAlbumGainRounded}");
            Debug.WriteLine($"TRACK GAIN ROUNDED: {this.ReplayTrackGainRounded}");
            Debug.WriteLine($"FILE LENGTH: {this.Length}");
        }

        internal void ExtractTags()
        {
            if (!this.HasTags)
            {
                try
                {
                    var info = new FileInfo(this.FilePath);

                    this.Length = info.Length;
                }
                catch (Exception ex)
                {
                    this.Length = 0L;

                    GenerateErrorMessage("File Info Error", ex);
                }

                try
                {
                    var tagFile = TagLib.File.Create(this.FilePath);
                    if (tagFile.Tag.Performers.Length > 0) { this.Artist = tagFile.Tag.Performers[0]; }
                    var types = tagFile.TagTypes;
                    var tags = tagFile.GetTag(TagTypes.Ape);

                    if (tags is TagLib.Ape.Tag apeTag)
                    {
                        GetTagValuePair(apeTag, TagMp3GainAlbumMinMax, out GainAlbumMin, out GainAlbumMax);

                        GetTagValuePair(apeTag, TagMp3GainMinMax, out GainMin, out GainMax);
                        GetTagValueTriple(apeTag, TagMp3GainUndo, out GainUndoTrack, out GainUndoAlbum, out GainUndoLabel);
                        GetTagValue(apeTag, TagReplayAlbumGain, out this.ReplayAlbumGain);                        
                        GetTagValue(apeTag, TagReplayAlbumPeak, out this.ReplayAlbumPeak);
                        GetTagValue(apeTag, TagReplayTrackGain, out this.ReplayTrackGain);
                        GetTagValue(apeTag, TagReplayTrackPeak, out this.ReplayTrackPeak);
                    }

                    this.Album = tagFile.Tag.Album;
                    this.Title = tagFile.Tag.Title;
                    this.Track = tagFile.Tag.Track;
                    this.AlbumArtist = tagFile.Tag.FirstAlbumArtist;
                    this.Updated = true;
                    this.HasTags = true;

                    //WriteDebug();
                }
                catch (CorruptFileException ex)
                {
                    GenerateErrorMessage("Tag Extract Error", ex);
                }
                catch (IOException ex)
                {
                    GenerateErrorMessage("Still open in mp3gain Error", ex);
                }
            }
        }

        internal void UpdateTags()
        {
            this.HasTags = false;
            ExtractTags();
        }

        private static void GetTagValue(TagLib.Ape.Tag apeTag, string key, out double value)
        {
            value = -1.0;

            if (apeTag.HasItem(key))
            {
                var item = apeTag.GetItem(key);
                var vector = item.ToString().Split(',');

                Double.TryParse(vector[0].Replace(" dB", string.Empty), out value);
            }
        }

        private static void GetTagValuePair(TagLib.Ape.Tag apeTag, string key, out double value0, out double value1)
        {
            value0 = -1.0;
            value1 = -1.0;

            if (apeTag.HasItem(key))
            {
                var item = apeTag.GetItem(key);
                var vector = item.ToString().Split(',');

                Double.TryParse(vector[0], out value0);
                Double.TryParse(vector[1], out value1);
            }
        }

        private void GenerateErrorMessage(string header, Exception ex)
        {
            var message = $"{header}: {ex.Message}";
            this.ErrorMessages.Add(message);

            Debug.WriteLine($"{message} ({this.FilePath})");
        }

        private void GetTagValueTriple(TagLib.Ape.Tag apeTag, string key, out double value0, out double value1, out string label)
        {
            value0 = -1.0;
            value1 = -1.0;
            label = string.Empty;

            if (apeTag.HasItem(key))
            {
                var item = apeTag.GetItem(key);
                var vector = item.ToString().Split(',');

                Double.TryParse(vector[0], out value0);
                Double.TryParse(vector[1], out value1);
                label = vector[2];
            }
        }
    }
}