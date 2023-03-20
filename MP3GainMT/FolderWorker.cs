using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinFormMP3Gain;

namespace MP3GainMT
{
    internal class FolderWorker
    {
        public BackgroundWorker Worker;
        public MP3GainFolder Folder;

        public FolderWorker(BackgroundWorker worker, MP3GainFolder folder)
        {
            Worker = worker;
            Folder = folder;
        }
    }
}
