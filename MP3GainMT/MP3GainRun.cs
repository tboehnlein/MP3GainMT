using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.Automation;

namespace WinFormMP3Gain
{
    internal class MP3GainRun
    {
        public event EventHandler<MP3GainFolder> FolderFinished;
        public event EventHandler<MP3GainFile> FoundFile;
        public event EventHandler<MP3GainFile> ChangedFile;
        public event EventHandler<MP3GainFolder> SearchFinished;
        public event EventHandler RefreshTable;

        private Dictionary<string, BackgroundWorker> workers = new Dictionary<string, BackgroundWorker>();
        private DateTime lastRefresh = DateTime.Now;

        public Dictionary<string, MP3GainFolder> Folders { get; set; } = new Dictionary<string, MP3GainFolder>();

        public static string Executable { get; set; } = string.Empty;

        public string ParentFolder { get; } = string.Empty;
        public int FoldersLeft { get; private set; }

        public MP3GainRun(string exePath, string parentFolder)
        {
            if (Executable == string.Empty)
            {
                Executable = exePath;
            }

            ParentFolder = parentFolder;
        }

        public void SearchFolders()
        {
            this.FindFolders(this.ParentFolder);
        }

        public void Run()
        {
            this.ProcessFolders();
        }

        public void ProcessFolders()
        {
            this.FoldersLeft = Folders.Values.Count;

            foreach (var folder in Folders.Values)
            {
                var worker = new BackgroundWorker();
                worker.WorkerReportsProgress = true;

                worker.DoWork += Worker_DoWork;
                worker.ProgressChanged += Worker_ProgressChanged;
                worker.RunWorkerCompleted += Worker_RunWorkerCompleted;

                worker.RunWorkerAsync(folder);
            }
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if ((DateTime.Now - this.lastRefresh).TotalSeconds > .250)
            {
                this.RaiseRefreshTable();
                this.lastRefresh = DateTime.Now;
            }
        }

        private void RaiseRefreshTable()
        {
            if (this.RefreshTable != null)
            {
                this.RefreshTable.Invoke(this, null);
            }
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result is MP3GainFolder folder)
            {
                string fileWord = WordWithEnding("file", folder.MP3Files);
                Debug.WriteLine($"{folder.FolderName} is done. Gain used {folder.SuggestedGain} for {folder.MP3Files.Count} {fileWord}.");
                this.RaiseFolderFinished(this, folder);
            }
        }

        private void RaiseFolderFinished(object sender, MP3GainFolder folder)
        {
            this.FoldersLeft--;

            if (this.FolderFinished != null)
            {
                this.FolderFinished.Invoke(sender, folder);
            }            
        }

        public static string WordWithEnding<T>(string word, List<T> list)
        {
            return word + (list.Count == 0 ? "" : "s");
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (e.Argument is MP3GainFolder folder)
            {
                folder.RunFolder(Executable, sender as BackgroundWorker);
                e.Result = folder;
            }
        }

        private void FindFolders(string parentFolder)
        {
            var folders = Directory.GetDirectories(parentFolder, "*", SearchOption.AllDirectories).ToList();

            folders.Add(parentFolder);

            foreach (var folder in folders)
            {
                var mp3Folder = new MP3GainFolder(folder);

                mp3Folder.FoundFile += MP3Folder_FoundFile;
                mp3Folder.ChangedFile += MP3Folder_ChangedFile;
                mp3Folder.SearchFolder();

                if (mp3Folder.MP3Files.Count > 0)
                {
                    this.Folders.Add(folder, mp3Folder);
                    this.RaiseSearchFinished(mp3Folder);
                }
            }
        }

        private void MP3Folder_ChangedFile(object sender, MP3GainFile e)
        {
            this.RaiseChangedFile(sender, e);
        }

        private void RaiseChangedFile(object sender, MP3GainFile e)
        {
            if (this.ChangedFile != null)
            {
                this.ChangedFile.Invoke(sender, e);
            }
        }

        private void RaiseSearchFinished(MP3GainFolder mp3Folder)
        {
            if (this.SearchFinished != null)
            {
                this.SearchFinished.Invoke(this, mp3Folder);
            }
        }

        private void MP3Folder_FoundFile(object sender, MP3GainFile e)
        {
            this.RaiseFoundFile(sender, e);
        }

        private void RaiseFoundFile(object sender, MP3GainFile file)
        {
            if (this.FoundFile != null)
            {
                this.FoundFile.Invoke(sender, file);
            }
        }
    }
}
