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
using System.Diagnostics;
using System.IO;
using System.Linq;
using TagLib;
using TagLib.Id3v2;

namespace MP3GainMT.MP3Gain
{
    public class Mp3File
    {
        public const double FiveLog10Two = 1.50514997831991;
        public const string TagMp3GainAlbumMinMax = "MP3GAIN_ALBUM_MINMAX";
        public const string TagMp3GainMinMax = "MP3GAIN_MINMAX";
        public const string TagMp3GainUndo = "MP3GAIN_UNDO";
        public const string TagReplayAlbumGain = "REPLAYGAIN_ALBUM_GAIN";
        public const string TagReplayAlbumPeak = "REPLAYGAIN_ALBUM_PEAK";
        public const string TagReplayTrackGain = "REPLAYGAIN_TRACK_GAIN";
        public const string TagReplayTrackPeak = "REPLAYGAIN_TRACK_PEAK";
        public double GainAlbumMax = 0.0;
        public double GainMax = 0.0;
        public double GainUndoAlbum = 0.0;
        public string GainUndoLabel = string.Empty;
        public double GainUndoTrack = 0.0;
        public double MaxNoClipGainAlbum = 0.0;
        public double MaxNoClipGainTrack = 0.0;
        public double ReplayAlbumGain = 0.0;
        public double ReplayAlbumPeak = 0.0;
        public double ReplayTrackGain = 0.0;
        public double ReplayTrackPeak = 0.0;
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

        public bool HasGainTags { get; private set; } = false;

        public bool HasTags { get; internal set; } = false;

        public long Length { get; private set; }

        public double MaxNoClipGainAlbumRaw { get; internal set; } = 0.0;

        public double MaxNoClipGainTrackRaw { get; internal set; } = 0.0;

        public int Progress { get; internal set; } = 0;

        public double ReplayAlbumGainRounded
        {
            get
            {
                var gain = DbRounding(ReplayAlbumGain);

                return gain;
            }
        }

        public double ReplayTrackGainRounded
        {
            get
            {
                var gain = DbRounding(ReplayTrackGain);

                return gain;
            }
        }

        public int SourceIndex { get; internal set; }

        public int SuggestedAlbumGain => Convert.ToInt32(Math.Round(ReplayAlbumGain / FiveLog10Two));

        public double SuggestedGain { get; set; } = 0;

        public string Title { get; set; } = string.Empty;

        public uint Track { get; private set; }

        public bool Updated { get; internal set; }

        public bool UseAlternativeColor { get; internal set; }

        public static double DbRounding(double x)
        {
            return Math.Round(x / FiveLog10Two) * FiveLog10Two;
        }

        public static double GainRounding(double x)
        {
            return Math.Floor(x / FiveLog10Two) * FiveLog10Two;
        }

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
            Debug.WriteLine($"MIN: {this.MaxNoClipGainTrack} MAX: {this.GainMax}");
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
                    var apeTags = tagFile.GetTag(TagTypes.Ape);
                    var id3Tags = tagFile.GetTag(TagTypes.Id3v2);

                    if (apeTags is TagLib.Ape.Tag apeTag)
                    {
                        GetTagValueTriple(apeTag, TagMp3GainUndo, out GainUndoTrack, out GainUndoAlbum, out GainUndoLabel);
                        var hasAll = GetTagValue(apeTag, TagReplayAlbumGain, out this.ReplayAlbumGain);
                        GetTagValue(apeTag, TagReplayAlbumPeak, out this.ReplayAlbumPeak);
                        hasAll &= GetTagValue(apeTag, TagReplayTrackGain, out this.ReplayTrackGain);
                        GetTagValue(apeTag, TagReplayTrackPeak, out this.ReplayTrackPeak);

                        this.HasGainTags = hasAll;
                    }
                    else if (id3Tags is TagLib.Id3v2.Tag id3v2Tag)
                    {
                        //var execute = new ExecuteMp3GainSync(Executable,
                        //                         $"/o /s c",
                        //                         folder.Files,
                        //                         folder.FolderPath,
                        //                         "GET MAX GAIN",
                        //                         "Calculating Max Gain",
                        //                         "done");

                        //execute.Execute();

                        // TODO: write code that runs mp3gain /o /s /c and extracts
                        // the tags values from the file using the output rather than
                        // the taglib library which currently does not support id3v2 properly
                        // if lyrics tags are in the the tag list where APE tag is after it
                        // This is the backup procedure

                        this.ErrorMessages.Add($"File does not contain gain information.");
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

        internal void FlagUpdateTags()
        {
            this.HasTags = false;
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
        private static bool GetTagValue(TagLib.Ape.Tag apeTag, string key, out double value)
        {
            var hasTag = apeTag.HasItem(key);

            value = -1.0;

            if (hasTag)
            {
                var item = apeTag.GetItem(key);
                var vector = item.ToString().Split(',');

                Double.TryParse(vector[0].Replace(" dB", string.Empty), out value);
            }

            return hasTag;
        }

        private static bool GetTagValuePair(TagLib.Ape.Tag apeTag, string key, out double value0, out double value1)
        {
            var hasTag = apeTag.HasItem(key);

            value0 = -1.0;
            value1 = -1.0;

            if (hasTag)
            {
                var item = apeTag.GetItem(key);
                var vector = item.ToString().Split(',');

                Double.TryParse(vector[0], out value0);
                Double.TryParse(vector[1], out value1);
            }

            return hasTag;
        }

        private void GenerateErrorMessage(string header, Exception ex)
        {
            var message = $"{header}: {ex.Message}";
            this.ErrorMessages.Add(message);

            Debug.WriteLine($"{message} ({this.FilePath})");
        }

        private bool GetTagValueTriple(TagLib.Ape.Tag apeTag, string key, out double value0, out double value1, out string label)
        {
            var hasTag = apeTag.HasItem(key);
            value0 = -1.0;
            value1 = -1.0;
            label = string.Empty;

            if (hasTag)
            {
                var item = apeTag.GetItem(key);
                var vector = item.ToString().Split(',');

                Double.TryParse(vector[0], out value0);
                Double.TryParse(vector[1], out value1);
                label = vector[2];
            }

            return hasTag;
        }

        private bool GetTagValueTriple(TagLib.Id3v2.Tag id3v1Tag, string key, out double value0, out double value1, out string label)
        {


            //var list = id3v2Tag.GetFrames(key);
            //var fields = list.Select(x => x.FrameId).ToList();
            //var hasTag = list.ToList().Exists(x => x. == key);
            value0 = -1.0;
            value1 = -1.0;
            label = string.Empty;

            //if (hasTag)
            //{
            //    var item = apeTag.GetItem(key);
            //    var vector = item.ToString().Split(',');

            //    Double.TryParse(vector[0], out value0);
            //    Double.TryParse(vector[1], out value1);
            //    label = vector[2];
            //}

            var hasTag = false;

            return hasTag;
        }
    }
}