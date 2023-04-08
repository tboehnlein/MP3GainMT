﻿using System.ComponentModel;

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