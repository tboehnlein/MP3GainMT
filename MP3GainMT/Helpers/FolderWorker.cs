using MP3GainMT.MP3Gain;
using System.ComponentModel;

namespace MP3GainMT
{
    public class FolderWorker
    {
        public Mp3Folder Folder;
        public BackgroundWorker Worker;
        public FolderWorker(BackgroundWorker worker, Mp3Folder folder)
        {
            Worker = worker;
            Folder = folder;
        }
    }
}