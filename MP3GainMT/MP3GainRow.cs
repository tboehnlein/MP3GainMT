using System;
using WinFormMP3Gain;

namespace MP3GainMT
{
    internal class MP3GainRow : IComparable<MP3GainRow>, IEquatable<MP3GainRow>
    {
        public string FullPath => this.file.FilePath;
        public string Folder => this.file.Folder;
        public string FileName => this.file.FileName;
        public double TrackDB => Math.Round(89.0 - this.file.DBOffset, 1);
        public double TrackFinal => Math.Round(this.TrackDB + this.folder.SuggestedGain, 1);

        public int Progress => this.file.Progress;

        private MP3GainFile file = null;
        private MP3GainFolder folder;

        public double AlbumDB => Math.Round(89.0 - this.folder.DBOffset, 1);
        public double AlbumFinal => Math.Round(AlbumDB + this.folder.SuggestedGain, 1);

        public MP3GainRow(MP3GainFile file, MP3GainFolder folder)
        {
            this.file = file;
            this.folder = folder;
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

        public int CompareTo(MP3GainRow row)
        {
            if (row == null) return 1;

            if (ReferenceEquals(this, row)) return 0;

            return this.FullPath.CompareTo(row.FullPath);
        }
    }
}