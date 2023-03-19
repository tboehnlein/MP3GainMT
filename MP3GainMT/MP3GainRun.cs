﻿using MP3GainMT;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace WinFormMP3Gain
{
    internal class MP3GainRun
    {
        public event EventHandler<MP3GainFolder> FolderFinished;

        public event EventHandler<MP3GainFile> FoundFile;

        public event EventHandler<MP3GainFile> ChangedFile;

        public event EventHandler<MP3GainFolder> SearchFinishedFolder;

        public event EventHandler RefreshTable;

        public event EventHandler<bool> UpdateSearchProgress;

        private Dictionary<string, BackgroundWorker> workers = new Dictionary<string, BackgroundWorker>();
        private DateTime lastRefresh = DateTime.Now;

        private Dictionary<string, MP3GainFile> foundFiles = new Dictionary<string, MP3GainFile>();
        private List<MP3GainFolder> finished = new List<MP3GainFolder>();

        public BindingList<MP3GainRow> DataSource { get; private set; } = new BindingList<MP3GainRow>();

        public Dictionary<string, MP3GainFolder> Folders { get; set; } = new Dictionary<string, MP3GainFolder>();

        public static string Executable { get; set; } = string.Empty;

        public string ParentFolder { get; set; } = string.Empty;
        public int FoldersLeft { get; private set; }

        public MP3GainRun(string exePath, string parentFolder = "")
        {
            if (Executable == string.Empty)
            {
                Executable = exePath;
            }

            ParentFolder = parentFolder;
        }

        public void SearchFolders(string parentFolder = "")
        {
            var worker = new BackgroundWorker();

            this.ParentFolder = parentFolder;

            worker.WorkerReportsProgress = true;

            worker.DoWork += SearchWorker_DoWork;
            worker.ProgressChanged += SearchWorker_ProgressChanged;
            worker.RunWorkerCompleted += SearchWorker_RunWorkerCompleted;

            worker.RunWorkerAsync(this.ParentFolder);
        }

        private void SearchWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.RaiseRefreshTable();
        }

        private void SearchWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.RaiseUpdateSearchProgress(true);
        }

        private void SearchWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (e.Argument is string folder && sender is BackgroundWorker worker)
            {
                this.SourceDictionary.Clear();
                this.FindFolders(folder, worker);
                worker.ReportProgress(100);
            }
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
                var index = Folders.Values.ToList().IndexOf(folder);
                var worker = new BackgroundWorker();
                worker.WorkerReportsProgress = true;

                worker.DoWork += ProcessWorker_DoWork;
                worker.ProgressChanged += ProcessWorker_ProgressChanged;
                worker.RunWorkerCompleted += ProcessWorker_RunWorkerCompleted;

                worker.RunWorkerAsync(folder);

                /*if (index != 0 && index % 4 == 0)
                {
                    while(!this.IsFolderFinished(folder))
                    {
                        Thread.Sleep(250);
                    }
                }*/
            }
        }

        private bool IsFolderFinished(MP3GainFolder folder)
        {
            return this.finished.Contains(folder);
        }

        private void ProcessWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
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

        private void ProcessWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result is MP3GainFolder folder)
            {
                string fileWord = WordWithEnding("file", folder.MP3Files);
                Debug.WriteLine($"{folder.FolderName} is done. Gain used {folder.SuggestedGain} for {folder.MP3Files.Count} {fileWord}.");
                this.RaiseFolderFinished(this, folder);
                this.finished.Add(folder);
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

        private void ProcessWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (e.Argument is MP3GainFolder folder)
            {
                folder.RunFolder(Executable, sender as BackgroundWorker);
                e.Result = folder;
            }
        }

        private void FindFolders(string parentFolder, BackgroundWorker worker)
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

                    if (folders.Count < 25)
                    {
                        this.RaiseSearchFinished(mp3Folder);
                    }

                    worker.ReportProgress(Convert.ToInt32((((double)folder.IndexOf(folder) + 1) / folders.Count) * 100.0));
                }
            }
        }

        private void RaiseUpdateSearchProgress(bool force = false)
        {
            if (this.UpdateSearchProgress != null)
            {
                this.UpdateSearchProgress.Invoke(this, force);
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
            if (this.SearchFinishedFolder != null)
            {
                this.SearchFinishedFolder.Invoke(this, mp3Folder);
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

        internal void Clear()
        {
            this.foundFiles.Clear();
            this.DataSource.Clear();
            this.Folders.Clear();
            this.finished.Clear();
        }

        internal void RefreshDataSource()
        {
            this.RaiseUpdateSearchProgress();
            this.RefreshDataSource(AllFiles);
        }

        private List<MP3GainFile> AllFiles => this.foundFiles.Select(x => x.Value).ToList();

        public Dictionary<string, MP3GainRow> SourceDictionary { get; private set; } = new Dictionary<string, MP3GainRow>();

        public int FileCount
        {
            get { return this.foundFiles.Count; }
        }

        internal void RefreshDataSource(List<MP3GainFile> folderFiles)
        {
            foreach (var file in folderFiles)
            {
                UpdateFile(file);
            }
        }

        public void UpdateFile(MP3GainFile file)
        {
            if (this.Folders.ContainsKey(file.FolderPath))
            {
                if (!this.SourceDictionary.ContainsKey(file.FilePath))
                {
                    var row = new MP3GainRow(file, this.Folders[file.FolderPath]);
                    this.SourceDictionary.Add(file.FilePath, row);
                    this.DataSource.Add(row);
                }
            }
        }

        internal void AddFile(MP3GainFile file)
        {
            if (!this.foundFiles.ContainsKey(file.FilePath))
            {
                this.foundFiles.Add(file.FilePath, file);
            }
            else
            {
                UpdateFile(file);
            }
        }

        internal void UpdateFolder(MP3GainFolder e)
        {
            var folderFiles = FolderFiles(e);

            if (folderFiles.Any())
            {
                this.RefreshDataSource(folderFiles);
            }
        }

        public List<MP3GainFile> FolderFiles(MP3GainFolder folder)
        {
            return folder.Files.Select(x => x.Value).ToList();
        }
    }
}