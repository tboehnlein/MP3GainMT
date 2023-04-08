﻿using System;

namespace MP3GainMT
{
    public class MP3GainRow : IComparable<MP3GainRow>, IEquatable<MP3GainRow>
    {
        public const double TargetDefault = 89.0;
        private MP3GainFile file = null;

        private MP3GainFolder folder;

        public MP3GainRow(MP3GainFile file, MP3GainFolder folder)
        {
            this.file = file;
            this.folder = folder;
        }

        public static double TargetDB { get; set; } = 89.0;
        public string Album => this.file.Album;
        public string AlbumArtist => this.file.AlbumArtist;
        public bool AlbumClipping => this.file.AlbumClipping;
        public double AlbumDB => Math.Round(MP3GainRow.TargetDefault - this.file.ReplayAlbumGain, 1);
        public double AlbumFinal => Math.Round(this.file.ReplayAlbumGainRounded - TargetDifference, 1);
        public string Artist => this.file.Artist;
        public string ErrorMessage => this.file.ErrorMessages.AsSingleLine();
        public string FileName => this.file.FileName;
        public string Folder => this.file.Folder;
        public string FullPath => this.file.FilePath;
        public int Progress { get; set; } = 0;
        public bool TrackClipping => this.file.TrackClipping;
        public double TrackDB => Math.Round(MP3GainRow.TargetDefault - this.file.ReplayTrackGain, 1);
        public double TrackFinal => Math.Round(this.file.ReplayTrackGainRounded - TargetDifference, 1);

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

        private static double TargetDifference => MP3GainRow.TargetDefault - MP3GainRow.TargetDB;
        public int CompareTo(MP3GainRow row)
        {
            if (row == null) return 1;

            if (ReferenceEquals(this, row)) return 0;

            return this.FullPath.CompareTo(row.FullPath);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as MP3GainRow);
        }

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

        public override int GetHashCode()
        {
            return FullPath.GetHashCode() + AlbumFinal.GetHashCode();
        }
    }
}