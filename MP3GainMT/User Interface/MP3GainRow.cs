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

using MP3GainMT.MP3Gain;
using System;

namespace MP3GainMT
{
    // WARNING: DO NOT REORGANIZE THIS FILE WITH CODEMAID. Public property order affects the column order of the table.

    /// <summary>
    /// A class that represents a row in the MP3Gain table. This determines the columns and their order. It does 
    /// all of the final rounding and calculations for the table.  This ensure that the table matches the output
    /// of the original MP3Gain software.
    /// </summary>
    public class MP3GainRow : IComparable<MP3GainRow>, IEquatable<MP3GainRow>
    {
        public const double TargetDefault = 89.0;
        private Mp3File file = null;

        private Mp3Folder folder;

        /// <summary>
        /// Creates a new MP3GainRow object that contains a MP3 and belongs to a folder.
        /// </summary>
        /// <param name="file">The MP3 file associated with the row.</param>
        /// <param name="folder">The folder that holds the MP3 file.</param>
        public MP3GainRow(Mp3File file, Mp3Folder folder)
        {
            this.file = file;
            this.folder = folder;
        }

        public bool HasGainTags => this.file.HasGainTags;

        /// <summary>
        /// The full file path of the MP3 file.
        /// </summary>
        public string FullPath => this.file.FilePath;

        /// <summary>
        /// The target volume in decibels.
        /// </summary>
        public static double TargetDB { get; set; } = 89.0;

        /// <summary>
        /// The difference between the target volume and the default volume.
        /// </summary>
        public static double TargetDiffDB => TargetDefault - TargetDB;

        /// <summary>
        /// The tagged album artist of the MP3 file.
        /// </summary>
        public string AlbumArtist => this.file.AlbumArtist;

        /// <summary>
        /// The tagged artist of the MP3 file.
        /// </summary>
        public string Artist => this.file.Artist;

        /// <summary>
        /// The tagged album of the MP3 file.
        /// </summary>
        public string Album => this.file.Album;

        /// <summary>
        /// The path of the MP3 file's folder.
        /// </summary>
        public string Folder => this.file.Folder;

        /// <summary>
        /// The MP3 file's file name
        /// </summary>
        public string FileName => this.file.FileName;

        /// <summary>
        /// The progress of the MP3 file's analysis.
        /// </summary>
        public int Progress { get; set; } = 0;

        /// <summary>
        /// The MP3 files's track volumn in decibels relative to the target decibels.
        /// </summary>
        public double TrackDB => Math.Round(TargetDB - this.file.ReplayTrackGain, 1);

        /// <summary>
        /// Is that track clipping in it's current volume state?
        /// </summary>
        public bool Clipping => this.file.MaxNoClipGainTrack < 0.0;

        /// <summary>
        /// Difference between the track's gain and the target's volume in decibels.
        /// </summary>
        public double TrackGain => Math.Round(Mp3File.DbRounding(this.file.ReplayTrackGain - TargetDiffDB), 1);

        /// <summary>
        /// Would the track clip if it were to have track gain applied?
        /// </summary>
        public bool TrackClipping => this.file.MaxNoClipGainTrack < TrackGain;

        /// <summary>
        /// The MP3 files's album volumn in decibels relative to the target decibels.
        /// </summary>
        public double AlbumDB => Math.Round(TargetDB - this.file.ReplayAlbumGain, 1);

        /// <summary>
        /// Difference between the album's gain and the target's volume in decibels.
        /// </summary>
        public double AlbumGain => Math.Round(Mp3File.DbRounding(this.file.ReplayAlbumGain - TargetDiffDB), 1);

        /// <summary>
        /// Would the track clip if it were to have album gain applied?
        /// </summary>
        public bool AlbumClipping => this.file.MaxNoClipGainTrack < AlbumGain;

        public bool AlbumColorAlternative
        {
            get
            {
                return this.file.UseAlternativeColor;
            }

            set
            {
                this.file.UseAlternativeColor = value;
            }
        }

        /// <summary>
        /// The amount of gain that could be applied to the track without clipping.
        /// </summary>
        public double MaxNoClip => Math.Round(Mp3File.GainRounding(this.file.MaxNoClipGainTrack), 1);

        /// <summary>
        /// Error messages associated with the MP3 file.
        /// </summary>
        public string ErrorMessage => $"{this.file.ErrorMessages.AsSingleLine()}";

        /// <summary>
        /// Has the file been updated?
        /// </summary>
        public bool Updated
        {
            get
            {
                return this.file.Updated;
            }
            set
            {
                this.file.Updated = value;
            }
        }

        /// <summary>
        /// Difference between the default volume and the target volume.
        /// </summary>
        private static double TargetDifference => MP3GainRow.TargetDefault - MP3GainRow.TargetDB;

        /// <summary>
        /// Checks ot see if two MP3GainRow objects are equal.
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public int CompareTo(MP3GainRow row)
        {
            if (row == null) return 1;

            if (ReferenceEquals(this, row)) return 0;

            return this.FullPath.CompareTo(row.FullPath);
        }

        /// <summary>
        /// Compares to MP3GainRow to an object to see if they are equal.
        /// </summary>
        /// <param name="obj">An MP3GainRow.</param>
        /// <returns>True is equal else false.</returns>
        public override bool Equals(object obj)
        {
            return this.Equals(obj as MP3GainRow);
        }

        /// <summary>
        /// Compares to MP3GainRow objects to see if they are equal.
        /// </summary>
        /// <param name="obj">An MP3GainRow.</param>
        /// <returns>True is equal else false.</returns>
        public bool Equals(MP3GainRow obj)
        {
            if (obj == null) return false;

            if (ReferenceEquals(this, obj)) return true;

            if (obj.GetType() != GetType()) return false;

            if (obj is MP3GainRow objRow)
            {
                return this.FullPath.Equals(objRow.FullPath);
            }

            return false;
        }

        /// <summary>
        /// Generates a hash code for the MP3GainRow object using path and album gain.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return FullPath.GetHashCode() + AlbumGain.GetHashCode();
        }
    }
}