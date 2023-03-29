using MP3GainMT;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
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

        public event EventHandler<MP3GainFolder> FolderLoaded;

        public event EventHandler<int> TaskProgressed;

        public event EventHandler<int> RowUpdated;

        public event EventHandler<MP3GainFile> TagRead;

        public event EventHandler<TimeSpan> SearchTimeElasped;
        public event EventHandler<string> ActivityUpdated;

        private System.Windows.Forms.Timer searchTimeElaspedTimer = null;

        private void RaiseSearchTimeElapsed()
        {
            if (SearchTimeElasped != null)
            {
                SearchTimeElasped.Invoke(this, this.ElaspedSearchTime);
            }
        }

        private void RaiseAcivityUpdated(string message)
        {
            if (ActivityUpdated != null)
            {
                ActivityUpdated.Invoke(this, message);
            }
        }

        private Dictionary<string, BackgroundWorker> workers = new Dictionary<string, BackgroundWorker>();
        private DateTime lastRefresh = DateTime.Now;

        private Dictionary<string, MP3GainFile> foundFiles = new Dictionary<string, MP3GainFile>();
        private List<MP3GainFolder> finished = new List<MP3GainFolder>();
        private int runningInBG;
        private Stack<FolderWorker> processQueue;
        private DateTime startSearchTime;

        private TimeCheck findFileEventCheck = new TimeCheck(8);
        private TimeCheck readTagEventCheck = new TimeCheck(8);

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
            searchTimeElaspedTimer.Interval = 250;

            this.processQueue = new Stack<FolderWorker>();
        }

        private void FindFilesUpdateTimer_Tick(object sender, EventArgs e)
        {
            this.RaiseSearchTimeElapsed();
        }

        public void SearchFolders(string parentFolder = "")
        {
            var searchWorker = new BackgroundWorker();

            searchWorker.WorkerReportsProgress = true;

            searchWorker.DoWork += SearchWorker_DoWork;
            searchWorker.ProgressChanged += SearchWorker_ProgressChanged;
            searchWorker.RunWorkerCompleted += SearchWorker_RunWorkerCompleted;

            searchWorker.RunWorkerAsync(parentFolder);
        }

        private void SearchWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.RaiseRefreshTable();
        }

        private void SearchWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage == -1)
            {
                if (!this.searchTimeElaspedTimer.Enabled)
                {
                    this.StartSearchTimer();
                }
                else
                {
                    this.StopSearchTimer();
                    this.RaiseAcivityUpdated("Loading files");
                }
            }
            else
            {
                this.RaiseTaskProgressed(e.ProgressPercentage);

                if (e.UserState is MP3GainFolder folder)
                {
                    if (findFileEventCheck.CheckTime(e.ProgressPercentage == 100))
                    {
                        this.RaiseFolderLoaded(folder);
                        this.RaiseAcivityUpdated("Finished");
                    }
                }
            }
        }

        private void RaiseFolderLoaded(MP3GainFolder folder)
        {
            if (this.FolderLoaded != null)
            {
                this.FolderLoaded.Invoke(this, folder);
            }
        }

        private void StartSearchTimer()
        {
            this.startSearchTime = DateTime.Now;
            this.searchTimeElaspedTimer.Enabled = true;
            this.searchTimeElaspedTimer.Start();
        }

        private void StopSearchTimer()
        {
            this.searchTimeElaspedTimer.Stop();
            this.searchTimeElaspedTimer.Enabled = false;
        }

        private void SearchWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (e.Argument is string folder && sender is BackgroundWorker searchWorker)
            {
                this.FindFiles(folder, searchWorker);

                var progress = this.ContinueSearch ? 100 : 0;

                searchWorker.ReportProgress(progress, new TaskUpdate(progress, "Completed search.", -1));
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
                foreach (var file in AllFiles)
                {
                    file.Updated = false;
                }

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

        private void FindFiles(string parentFolder, BackgroundWorker searchWorker)
        {
            this.ContinueSearch = true;

            searchWorker.ReportProgress(-1);
            
            
            var folders = Directory.GetDirectories(parentFolder, "*", SearchOption.AllDirectories).ToList();
            var songs = Directory.GetFiles(parentFolder, "*.MP3", SearchOption.AllDirectories).ToList();

            searchWorker.ReportProgress(-1);

            folders.Add(parentFolder);

            var newFilesCount = this.GetNewFileCount(songs);

            if (newFilesCount > 25000)
            {
                var question = $"Are you sure you want to continue loading {newFilesCount} songs?";

                var result = MessageBox.Show(question, "MP3 Gain MT", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                this.ContinueSearch = (result == DialogResult.Yes);

                if (!this.ContinueSearch)
                {
                    return;
                }
            }

            var addedFilesCount = 0;

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
                        addedFilesCount += mp3Folder.MP3Files.Count;

                        var progress = Helpers.GetProgress(addedFilesCount, newFilesCount);
                        searchWorker.ReportProgress(progress, mp3Folder);
                    }
                }
            }
        }

        private int GetNewFileCount(List<string> filePaths)
        {
            return filePaths.Except(this.foundFiles.Keys).ToList().Count;
        }

        private void RaiseTaskProgressed(int progress)
        {
            if (this.TaskProgressed != null)
            {
                this.TaskProgressed.Invoke(this, progress);
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
            //this.RaiseUpdateSearchProgress(100);
            this.RefreshDataSource(AllFiles);
        }

        private List<MP3GainFile> AllFiles => this.foundFiles.Select(x => x.Value).ToList();

        public Dictionary<string, MP3GainRow> SourceDictionary { get; private set; } = new Dictionary<string, MP3GainRow>();

        public int FileCount
        {
            get { return this.foundFiles.Count; }
        }

        public TimeSpan ElaspedSearchTime => DateTime.Now - this.startSearchTime;

        public bool ContinueSearch { get; internal set; }

        public event EventHandler<string> AskSearchQuestion;

        internal void RefreshDataSource(List<MP3GainFile> folderFiles)
        {
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

        internal void ReadTags()
        {
            var readTagsWorker = new BackgroundWorker();

            readTagsWorker.WorkerReportsProgress = true;

            readTagsWorker.DoWork += ReadTagsWorker_DoWork;
            readTagsWorker.ProgressChanged += ReadTagsWorker_ProgressChanged;
            readTagsWorker.RunWorkerCompleted += ReadTagsWorker_RunWorkerCompleted;

            readTagsWorker.RunWorkerAsync();
        }

        private void ReadTagsWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.RaiseRefreshTable();
        }

        private void ReadTagsWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.RaiseTaskProgressed(e.ProgressPercentage);

            if (e.UserState is int index)
            {
                this.RaiseRowUpdated(index);

                if (this.readTagEventCheck.CheckTime(e.ProgressPercentage == 100))
                {
                    this.RaiseTagRead(this.AllFiles[index]);
                }
            }
        }

        private void RaiseTagRead(MP3GainFile mP3GainFile)
        {
            if (this.TagRead != null)
            {
                this.TagRead.Invoke(this, mP3GainFile);
            }
        }

        private void RaiseRowUpdated(int index)
        {
            if (this.RowUpdated != null)
            {
                this.RowUpdated.Invoke(this, index);
            }
        }

        private void ReadTagsWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (sender is BackgroundWorker worker)
            {
                var files = this.AllFiles;
                var filesDone = 1;
                var totalFiles = files.Count;

                worker.ReportProgress(0);

                foreach (var file in files)
                {
                    file.ExtractTags();
                    var progress = Helpers.GetProgress(filesDone, totalFiles);
                    worker.ReportProgress(progress, filesDone - 1);
                    filesDone++;
                }

                worker.ReportProgress(100);
            }
        }
    }
}