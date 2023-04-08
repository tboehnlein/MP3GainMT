using Equin.ApplicationFramework;
using MP3GainMT;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace MP3GainMT
{
    public class MP3GainRun
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

        private const string CancelMessage = "Cancellation successful.";
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

        private Dictionary<string, MP3GainFile> foundFiles = new Dictionary<string, MP3GainFile>();
        private List<MP3GainFolder> finished = new List<MP3GainFolder>();
        private Stack<FolderWorker> processQueue;
        private DateTime startSearchTime;

        private TimeCheck findFileEventCheck = new TimeCheck(8);
        private TimeCheck readTagEventCheck = new TimeCheck(30);
        private BackgroundWorker readTagsWorker;
        private BackgroundWorker searchWorker;
        private int filesDone;
        private BackgroundWorker undoGainWorker;

        public event EventHandler AnalysisFinished;

        public BindingListView<MP3GainRow> DataSource { get; private set; } = null;

        public BindingList<MP3GainRow> Source { get; private set; } = new BindingList<MP3GainRow>();

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

            this.DataSource = new BindingListView<MP3GainRow>(Source);

            this.processQueue = new Stack<FolderWorker>();
        }

        private void FindFilesUpdateTimer_Tick(object sender, EventArgs e)
        {
            this.RaiseSearchTimeElapsed();
        }

        public void SearchFolders(string parentFolder = "")
        {
            this.searchWorker = new BackgroundWorker();

            searchWorker.WorkerReportsProgress = true;
            searchWorker.WorkerSupportsCancellation = true;

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
                else if (e.UserState is string message)
                {
                    this.RaiseAcivityUpdated(message);
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
            this.ResetProgress();

            foreach (var folder in Folders.Values)
            {
                var index = Folders.Values.ToList().IndexOf(folder);
                var worker = new BackgroundWorker();
                worker.WorkerReportsProgress = true;

                worker.DoWork += ApplyGain_DoWork;
                worker.ProgressChanged += ExecuteMP3Gain_ProgressChanged;
                worker.RunWorkerCompleted += MP3GainExecute_RunWorkerCompleted;

                worker.RunWorkerAsync(folder);
            }
        }

        public void ProcessFiles()
        {
            this.ResetProgress();

            foreach (var folder in Folders.Values)
            {
                var index = Folders.Values.ToList().IndexOf(folder);
                var worker = new BackgroundWorker();
                worker.WorkerReportsProgress = true;
                worker.WorkerSupportsCancellation = true;

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
            this.RaiseTaskProgressed(e.ProgressPercentage);

            if (e.UserState is MP3GainFile file)
            {
                if (this.readTagEventCheck.CheckTime(e.ProgressPercentage == 100))
                {
                    this.DataSource[file.SourceIndex].Object.Progress = e.ProgressPercentage;

                    this.RaiseRowUpdated(file.SourceIndex);
                }
            }
            else if (e.UserState is string message)
            {
                this.RaiseAcivityUpdated(message);
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

        private void MP3GainExecute_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result is MP3GainFolder folder)
            {
                string fileWord = WordWithEnding("file", folder.MP3Files);

                Debug.WriteLine($"{folder.FolderName} is done. Gain used {folder.SuggestedGain} for {folder.MP3Files.Count} {fileWord}.");

                this.finished.Add(folder);
                this.FoldersLeft--;

                this.RaiseFolderFinished(this, folder);

                this.RunProgress = Helpers.GetProgress(this.finished.Count, this.Folders.Count);
                this.RaiseTaskProgressed(GetRunProgress(0));

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

            if (searchWorker.CancellationPending)
            {
                searchWorker.ReportProgress(100, CancelMessage);
                return;
            }

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

                        if (searchWorker.CancellationPending)
                        {
                            searchWorker.ReportProgress(100, CancelMessage);
                            break;
                        }
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
            this.Source = new BindingList<MP3GainRow>();
            this.DataSource = new BindingListView<MP3GainRow>(Source);
            this.Folders.Clear();
            this.finished.Clear();
            this.SourceDictionary.Clear();
            this.RaiseRefreshTable();
            this.filesDone = 0;
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
        public bool ActiveActivities => this.AnyWorkersActive();

        public double RunProgress { get; private set; }

        private bool AnyWorkersActive()
        {
            return ActiveWorker(this.readTagsWorker) || ActiveWorker(this.searchWorker);
        }

        public event EventHandler<string> AskSearchQuestion;

        internal void RefreshDataSource(List<MP3GainFile> folderFiles)
        {
            this.Source.RaiseListChangedEvents = false;

            foreach (var file in folderFiles)
            {
                UpdateFile(file);
            }

            this.Source.RaiseListChangedEvents = true;

            this.Source.ResetBindings();
        }

        public void UpdateFile(MP3GainFile file)
        {
            if (this.Folders.ContainsKey(file.FolderPath))
            {
                if (!this.SourceDictionary.ContainsKey(file.FilePath))
                {
                    var row = new MP3GainRow(file, this.Folders[file.FolderPath]);
                    this.SourceDictionary.Add(file.FilePath, row);
                    this.Source.Add(row);
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
            this.Source.RaiseListChangedEvents = false;
        }

        internal void ResumeDataSource()
        {
            this.Source.RaiseListChangedEvents = true;
            this.Source.ResetBindings();
        }

        internal void ReadTags()
        {
            this.readTagsWorker = new BackgroundWorker();

            readTagsWorker.WorkerReportsProgress = true;
            readTagsWorker.WorkerSupportsCancellation = true;

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

            if (e.UserState is MP3GainFile file)
            {
                this.RaiseRowUpdated(file.SourceIndex);

                if (this.readTagEventCheck.CheckTime(e.ProgressPercentage == 100))
                {
                    this.RaiseTagRead(file);
                }
            }
            else if (e.UserState is string message)
            {
                this.RaiseAcivityUpdated(message);
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
                this.filesDone = 0;
                var totalFiles = files.Count;

                worker.ReportProgress(0);

                foreach (var file in files)
                {
                    file.ExtractTags();
                    file.SourceIndex = this.filesDone;
                    var progress = Helpers.GetProgress(this.filesDone, totalFiles);
                    worker.ReportProgress(progress, file);
                    this.filesDone++;

                    if (worker.CancellationPending)
                    {
                        worker.ReportProgress(100, CancelMessage);
                        break;
                    }
                }

                worker.ReportProgress(100);
            }
        }

        internal void CancelActivity()
        {
            CancelWorker(this.readTagsWorker);
            CancelWorker(this.searchWorker);
        }

        private static void CancelWorker(BackgroundWorker worker)
        {
            if (worker is BackgroundWorker cancel && cancel.WorkerSupportsCancellation)
            {
                cancel.CancelAsync();
            }
        }

        private static bool ActiveWorker(BackgroundWorker worker)
        {
            bool active = false;

            if (worker is BackgroundWorker cancel)
            {
                active = cancel.IsBusy;
            }

            return active;
        }

        internal void UndoGain()
        {
            this.ResetProgress();

            foreach (var folder in Folders.Values)
            {
                var index = Folders.Values.ToList().IndexOf(folder);
                var worker = new BackgroundWorker();
                worker.WorkerReportsProgress = true;

                worker.DoWork += UndoGain_DoWork;
                worker.ProgressChanged += ExecuteMP3Gain_ProgressChanged;
                worker.RunWorkerCompleted += MP3GainExecute_RunWorkerCompleted;

                worker.RunWorkerAsync(folder);
            }
        }

        private void ResetProgress()
        {
            this.finished.Clear();
            this.RunProgress = 0;
            this.FoldersLeft = Folders.Values.Count;
        }

        private void ExecuteMP3Gain_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.RaiseTaskProgressed(GetRunProgress(e.ProgressPercentage));

            if (e.UserState is FileProgress fileProg)
            {
                if (this.readTagEventCheck.CheckTime(fileProg.Progress == 100))
                {
                    this.DataSource[fileProg.File.SourceIndex].Object.Progress = fileProg.Progress;
                    this.RaiseRowUpdated(fileProg.File.SourceIndex);
                }
            }
            else if (e.UserState is string message)
            {
                this.RaiseAcivityUpdated(message);
            }
        }

        private int GetRunProgress(int progressPercentage)
        {
            var progressSoFar = progressPercentage + this.RunProgress;

            return Convert.ToInt32(progressSoFar);
        }

        private void UndoGain_DoWork(object sender, DoWorkEventArgs e)
        {
            if (e.Argument is MP3GainFolder folder)
            {
                folder.UndoGain(Executable, sender as BackgroundWorker);
                e.Result = folder;
            }
        }

        /*private void UndoGainWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            
        }

        private void UndoGainWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            
        }

        private void UndoGainWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            
        }*/
    }
}