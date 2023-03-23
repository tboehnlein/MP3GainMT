using MP3GainMT;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Forms;

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

        public event EventHandler<TimeSpan> SearchTimeElasped;

        private System.Windows.Forms.Timer searchTimeElaspedTimer = null;

        private void TimerTick(object state)
        {
            this.RaiseSearchTimeElapsed();
        }

        private void RaiseSearchTimeElapsed()
        {
            if (SearchTimeElasped != null)
            {
                SearchTimeElasped.Invoke(this, this.ElaspedSearchTime);
            }
        }

        private Dictionary<string, BackgroundWorker> workers = new Dictionary<string, BackgroundWorker>();
        private DateTime lastRefresh = DateTime.Now;

        private Dictionary<string, MP3GainFile> foundFiles = new Dictionary<string, MP3GainFile>();
        private List<MP3GainFolder> finished = new List<MP3GainFolder>();
        private int runningInBG;
        private Stack<FolderWorker> processQueue;
        private DateTime startSearchTime;

        public event EventHandler AnalysisFinished;

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

            this.searchTimeElaspedTimer = new System.Windows.Forms.Timer();

            searchTimeElaspedTimer.Enabled = false;
            searchTimeElaspedTimer.Tick += FindFilesUpdateTimer_Tick;
            searchTimeElaspedTimer.Interval = 500;

            this.processQueue = new Stack<FolderWorker>();

        }

        private void FindFilesUpdateTimer_Tick(object sender, EventArgs e)
        {
            this.RaiseSearchTimeElapsed();
        }

        public void SearchFolders(string parentFolder = "")
        {
            var worker = new BackgroundWorker();

            worker.WorkerReportsProgress = true;

            worker.DoWork += SearchWorker_DoWork;
            worker.ProgressChanged += SearchWorker_ProgressChanged;
            worker.RunWorkerCompleted += SearchWorker_RunWorkerCompleted;

            worker.RunWorkerAsync(parentFolder);
        }

        private void SearchWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.RaiseRefreshTable();
        }

        private void SearchWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage == -1)
            {
                if (this.searchTimeElaspedTimer.Enabled)
                {
                    this.searchTimeElaspedTimer.Stop();
                }
                else
                {
                    this.startSearchTime = DateTime.Now;
                    this.searchTimeElaspedTimer.Start();
                }
            }
            else
            {
                this.RaiseUpdateSearchProgress(true);
            }
        }

        private void SearchWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (e.Argument is string folder && sender is BackgroundWorker worker)
            {
                this.FindFolders(folder, worker);
                worker.ReportProgress(100);
            }
        }

        public void ApplyGain()
        {
            this.ApplyFolderGain();
        }

        public void ApplyFolderGain()
        {
            this.FoldersLeft = Folders.Values.Count;

            foreach (var folder in Folders.Values)
            {
                var index = Folders.Values.ToList().IndexOf(folder);
                var worker = new BackgroundWorker();
                worker.WorkerReportsProgress = true;

                worker.DoWork += ApplyGain_DoWork;
                worker.ProgressChanged += ApplyGain_ProgressChanged;
                worker.RunWorkerCompleted += ApplyGain_RunWorkerCompleted;

                worker.RunWorkerAsync(folder);
            }
        }

        public void ProcessFiles()
        {
            /*var worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;

            worker.DoWork += QueueFiles_DoWork;
            worker.ProgressChanged += QueueFiles_ProgressChanged;
            worker.RunWorkerCompleted += QueueFiles_RunWorkerCompleted;

            worker.RunWorkerAsync();*/

            this.FoldersLeft = Folders.Values.Count;

            foreach (var folder in Folders.Values)
            {
                var index = Folders.Values.ToList().IndexOf(folder);
                var worker = new BackgroundWorker();
                worker.WorkerReportsProgress = true;

                worker.DoWork += ProcessFiles_DoWork;
                worker.ProgressChanged += ProcessFiles_ProgressChanged;
                worker.RunWorkerCompleted += ProcessFiles_RunWorkerCompleted;

                this.processQueue.Push(new FolderWorker(worker, folder));
            }

            RunFolderGroup();

        }

        private void RunFolderGroup()
        {
            for (int i = 0; i < 8; i++)
            {
                RunNextFolder();
            }
        }

        private void RunNextFolder()
        {
            if (processQueue.Count > 0)
            {
                var item = processQueue.Pop();
                item.Worker.RunWorkerAsync(item.Folder);
            }
        }

        private void ProcessFiles_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result is MP3GainFolder folder)
            {
                RunNextFolder();

                string fileWord = WordWithEnding("file", folder.MP3Files);
                Debug.WriteLine($"COMPLETE ANALYSIS FOR {folder.FolderName}. Gain used {folder.SuggestedGain} for {folder.MP3Files.Count} {fileWord}.");
                this.RaiseFolderFinished(this, folder);
                this.finished.Add(folder);
            }

            if (this.processQueue.Count == 0)
            {
                this.RaiseAnalysisFinished();
            }
        }

        private void RaiseAnalysisFinished()
        {
            if (this.AnalysisFinished != null)
            {
                this.AnalysisFinished.Invoke(this, EventArgs.Empty);
            }
        }

        private void ProcessFiles_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //if ((DateTime.Now - this.lastRefresh).TotalSeconds > .250)
            //{
            //    this.lastRefresh = DateTime.Now;
            //}
        }

        private void ProcessFiles_DoWork(object sender, DoWorkEventArgs e)
        {
            if (e.Argument is MP3GainFolder folder)
            {
                folder.ProcessFiles(Executable, sender as BackgroundWorker);
                e.Result = folder;
            }
        }

        private bool IsFolderFinished(MP3GainFolder folder)
        {
            return this.finished.Contains(folder);
        }

        private void ApplyGain_ProgressChanged(object sender, ProgressChangedEventArgs e)
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

        private void ApplyGain_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
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
                //this.FolderFinished.Invoke(sender, folder);
            }
        }

        public static string WordWithEnding<T>(string word, List<T> list)
        {
            return word + (list.Count == 0 ? "" : "s");
        }

        private void ApplyGain_DoWork(object sender, DoWorkEventArgs e)
        {
            if (e.Argument is MP3GainFolder folder)
            {
                folder.ApplyGainFolder(Executable, sender as BackgroundWorker);
                e.Result = folder;
            }
        }

        private void FindFolders(string parentFolder, BackgroundWorker worker)
        {
            worker.ReportProgress(-1);

            var folders = Directory.GetDirectories(parentFolder, "*", SearchOption.AllDirectories).ToList();

            worker.ReportProgress(-1);

            folders.Add(parentFolder);

            foreach (var folder in folders)
            {
                var mp3Folder = new MP3GainFolder(folder);

                mp3Folder.FoundFile += MP3Folder_FoundFile;
                mp3Folder.ChangedFile += MP3Folder_ChangedFile;
                mp3Folder.SearchFolder();

                if (mp3Folder.MP3Files.Count > 0)
                {
                    if (!this.Folders.ContainsKey(folder))
                    {
                        this.Folders.Add(folder, mp3Folder);

                        worker.ReportProgress(Convert.ToInt32((((double)folder.IndexOf(folder) + 1) / folders.Count) * 100.0));
                    }
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
            this.SourceDictionary.Clear();
            this.RaiseRefreshTable();
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

        public TimeSpan ElaspedSearchTime => DateTime.Now - this.startSearchTime;

        internal void RefreshDataSource(List<MP3GainFile> folderFiles)
        {
            //var gainRows = folderFiles.Select(x => new MP3GainRow(x, this.Folders[x.FilePath])).ToList();

            //this.DataSource. = gainRows;

            this.DataSource.RaiseListChangedEvents = false;

            foreach (var file in folderFiles)
            {
                UpdateFile(file);
            }

            this.DataSource.RaiseListChangedEvents = true;

            this.DataSource.ResetBindings();
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

        internal void SuspendDataSource()
        {
            this.DataSource.RaiseListChangedEvents = false;
        }

        internal void ResumeDataSource()
        {
            this.DataSource.RaiseListChangedEvents = true;
            this.DataSource.ResetBindings();
        }
    }
}