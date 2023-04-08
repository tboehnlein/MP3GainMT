using System.ComponentModel;

namespace MP3GainMT
{
    public class FolderWorker
    {
        public MP3GainFolder Folder;
        public BackgroundWorker Worker;
        public FolderWorker(BackgroundWorker worker, MP3GainFolder folder)
        {
            Worker = worker;
            Folder = folder;
        }
    }
}