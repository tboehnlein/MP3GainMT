using MP3GainMT.MP3Gain;
using System;

namespace MP3GainMT
{
    /// <summary>
    /// DO NOT REORGANIZE THIS FILE WITH CODEMAID. Public property order affects the column order of the table.
    /// </summary>

    public class MP3GainRow : IComparable<MP3GainRow>, IEquatable<MP3GainRow>
    {
        public const double TargetDefault = 89.0;
        private Mp3File file = null;

        private Mp3Folder folder;

        public MP3GainRow(Mp3File file, Mp3Folder folder)
        {
            this.file = file;
            this.folder = folder;
        }

        public string FullPath => this.file.FilePath;

        public static double TargetDB { get; set; } = 89.0;

        public string AlbumArtist => this.file.AlbumArtist;

        public string Artist => this.file.Artist;

        public string Album => this.file.Album;

        public string Folder => this.file.Folder;

        public string FileName => this.file.FileName;

        public int Progress { get; set; } = 0;

        public double TrackDB => Math.Round(TargetDB - this.file.ReplayTrackGain, 1);

        public bool Clipping => this.file.MaxNoClipGainTrack < 0.0;

        public double TrackFinal => Math.Round(Mp3File.DbRounding(this.file.ReplayTrackGain), 1);

        //TODO: Add code to calculate AlbumClipping using mp3gain track max gain value vs suggested track gain value (See programmer notes)
        public bool TrackClipping => this.file.MaxNoClipGainTrack < TrackFinal;

        public double AlbumDB => Math.Round(TargetDB - this.file.ReplayAlbumGain, 1);
        public double AlbumFinal => Math.Round(Mp3File.DbRounding(this.file.ReplayAlbumGain), 1);

        //TODO: Add code to calculate AlbumClipping using mp3gain album max gain value vs suggested album gain value (See programmer notes)

        // mp3Inf.CurrMaxAmp * 2# ^ (CDbl(mp3Inf.AlbumMp3Gain) / 4#) > 32767

        public bool AlbumClipping => this.file.MaxNoClipGainTrack < 0.0;

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

        public string ErrorMessage => this.file.ErrorMessages.AsSingleLine();// + this.file.MaxNoClipGainTrack.ToString("0.000");

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