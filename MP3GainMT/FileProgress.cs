namespace MP3GainMT
{
    public class FileProgress
    {
        public FileProgress(MP3GainFile activeFile, int progress)
        {
            this.File = activeFile;
            this.Progress = progress;
        }

        public MP3GainFile File { get; set; }
        public int Progress { get; set; }
    }
}