using MP3GainMT.MP3Gain;

namespace MP3GainMT
{
    public class FileProgress
    {
        public FileProgress(Mp3File activeFile, int progress)
        {
            this.File = activeFile;
            this.Progress = progress;
        }

        public Mp3File File { get; set; }
        public int Progress { get; set; }
    }
}