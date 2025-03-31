using Equin.ApplicationFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace MP3GainMT.MP3Gain
{
    public class Mp3GainManager
    {
        private const string CancelMessage = "Cancellation successful.";

        private int filesDone;

        private TimeCheck findFileEventCheck = new TimeCheck(8);

        private List<Mp3Folder> finished = new List<Mp3Folder>();

        private Dictionary<string, Mp3File> foundFiles = new Dictionary<string, Mp3File>();

        private Stack<FolderWorker> processStack;

        private TimeCheck readTagEventCheck = new TimeCheck(8);

        private BackgroundWorker readTagsWorker;

        private Queue<string> searchQeueu = new Queue<string>();
        private System.Windows.Forms.Timer searchTimeElaspedTimer = null;

        private BackgroundWorker searchWorker;

        private DateTime startSearchTime;

        public Mp3GainManager(string exePath, string parentFolder = "")
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

            this.processStack = new Stack<FolderWorker>();
        }

        public event EventHandler<string> ActivityUpdated;

        public event EventHandler AnalysisFinished;

        public event EventHandler<Mp3File> ChangedFile;

        public event EventHandler<Mp3Folder> FolderFinished;

        public event EventHandler<Mp3Folder> FolderLoaded;

        public event EventHandler<Mp3File> FoundFile;

        public event EventHandler RefreshTable;

        public event EventHandler<int> RowUpdated;

        public event EventHandler<Mp3Folder> SearchFinishedFolder;

        public event EventHandler<TimeSpan> SearchTimeElasped;

        public event EventHandler<Mp3File> TagRead;

        public event EventHandler<int> TaskProgressed;

        public static string Executable { get; set; } = string.Empty;
        public bool Active { get; private set; }
        public bool ActiveActivities => this.AnyWorkersActive();

        public List<Mp3Folder> AllFolders
        {
            get
            {
                var list = new List<Mp3Folder>();

                foreach (var folder in this.Folders.Values)
                {
                    list.Add(folder);
                }

                return list;
            }
        }

        public bool ContinueSearch { get; internal set; }
        public BindingListView<MP3GainRow> DataSource { get; private set; } = null;
        public TimeSpan ElaspedSearchTime => DateTime.Now - this.startSearchTime;

        public int FileCount
        {
            get { return this.foundFiles.Count; }
        }

        public Dictionary<string, Mp3Folder> Folders { get; set; } = new Dictionary<string, Mp3Folder>();
        public int FoldersLeft { get; private set; }
        public long Length { get; private set; } = 0;
        public string ParentFolder { get; set; } = string.Empty;

        public double RunProgress { get; private set; }

        public BindingList<MP3GainRow> Source { get; private set; } = new BindingList<MP3GainRow>();

        public Dictionary<string, MP3GainRow> SourceDictionary { get; private set; } = new Dictionary<string, MP3GainRow>();

        private List<Mp3File> AllFiles => this.foundFiles.Select(x => x.Value).ToList();

        public static string WordWithEnding<T>(string word, List<T> list)
        {
            return word + (list.Count == 0 ? "" : "s");
        }

        public void AnalyzeGain(int cores = 2)
        {
            this.ResetAnalysis();

            this.Active = true;

            var orderedList = Folders.Values.ToList();

            foreach (var folder in Folders.Values.Reverse())
            {
                var index = orderedList.IndexOf(folder);
                var worker = new BackgroundWorker();
                worker.WorkerReportsProgress = true;
                worker.WorkerSupportsCancellation = true;

                worker.DoWork += AnalyzeGain_DoWork;
                worker.ProgressChanged += ExecuteMP3Gain_ProgressChanged;
                worker.RunWorkerCompleted += ExecuteMP3Gain_RunWorkerCompleted;

                this.processStack.Push(new FolderWorker(worker, folder));

                Debug.WriteLine($"PUSHED[{this.processStack.Count}]: {folder.FolderName}");
            }

            RunFolderGroup(cores);
        }

        public void ApplyGain(int cores = 2)
        {
            this.ResetAnalysis();

            this.Active = true;

            var orderedList = Folders.Values.ToList();

            foreach (var folder in Folders.Values.Reverse())
            {
                var index = orderedList.IndexOf(folder);
                var worker = new BackgroundWorker();
                worker.WorkerReportsProgress = true;

                worker.DoWork += ApplyGain_DoWork;
                worker.ProgressChanged += ExecuteMP3Gain_ProgressChanged;
                worker.RunWorkerCompleted += ExecuteMP3Gain_RunWorkerCompleted;

                this.processStack.Push(new FolderWorker(worker, folder));

                Debug.WriteLine($"PUSHED[{this.processStack.Count}]: {folder.FolderName}");
            }

            this.RunFolderGroup(cores);
        }

        public List<Mp3File> FolderFiles(Mp3Folder folder)
        {
            return folder.Files.Select(x => x.Value).ToList();
        }

        public void SearchFolders(IEnumerable<string> parentFolders)
        {
            if (parentFolders.Count() == 0)
            {
                return;
            }

            if (this.searchQeueu.Count == 0)
            {
                this.searchQeueu = new Queue<string>(parentFolders);
            }

            this.searchWorker = CreateSearchWorker();

            this.searchWorker.RunWorkerAsync(this.searchQeueu.Dequeue());
        }

        public void SearchFolders(string parentFolder = "")
        {
            this.searchWorker = CreateSearchWorker();

            searchWorker.RunWorkerAsync(parentFolder);
        }

        public void UpdateFile(Mp3File file)
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

        internal void AddFile(Mp3File file)
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

        internal void CancelActivity()
        {
            CancelWorker(this.readTagsWorker);
            CancelWorker(this.searchWorker);
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

        internal void RefreshDataSource()
        {
            //this.RaiseUpdateSearchProgress(100);
            this.RefreshDataSource(AllFiles);
        }

        internal void RefreshDataSource(List<Mp3File> folderFiles)
        {
            this.Source.RaiseListChangedEvents = false;

            foreach (var file in folderFiles)
            {
                UpdateFile(file);
            }

            this.Source.RaiseListChangedEvents = true;

            this.Source.ResetBindings();
        }

        internal void ResumeDataSource()
        {
            this.Source.RaiseListChangedEvents = true;
            this.Source.ResetBindings();
        }

        internal void SuspendDataSource()
        {
            this.Source.RaiseListChangedEvents = false;
        }

        internal void UndoGain(int cores = 2)
        {
            this.ResetAnalysis();
            this.Active = true;

            var orderedList = Folders.Values.ToList();

            foreach (var folder in Folders.Values.Reverse())
            {
                var index = orderedList.IndexOf(folder);
                var worker = new BackgroundWorker();
                worker.WorkerReportsProgress = true;

                worker.DoWork += UndoGain_DoWork;
                worker.ProgressChanged += ExecuteMP3Gain_ProgressChanged;
                worker.RunWorkerCompleted += ExecuteMP3Gain_RunWorkerCompleted;

                this.processStack.Push(new FolderWorker(worker, folder));

                Debug.WriteLine($"PUSHED[{this.processStack.Count}]: {folder.FolderName}");
            }

            this.RunFolderGroup(cores);
        }

        internal void UpdateFolder(Mp3Folder e)
        {
            var folderFiles = FolderFiles(e);

            if (folderFiles.Any())
            {
                this.RefreshDataSource(folderFiles);
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

        private static void CancelWorker(BackgroundWorker worker)
        {
            if (worker is BackgroundWorker cancel && cancel.WorkerSupportsCancellation)
            {
                cancel.CancelAsync();
            }
        }

        private void AnalyzeGain_DoWork(object sender, DoWorkEventArgs e)
        {
            if (e.Argument is Mp3Folder folder)
            {
                folder.AnalyzeGainFolder(Executable, sender as BackgroundWorker);
                e.Result = folder;
            }
        }

        private bool AnyWorkersActive()
        {
            return ActiveWorker(this.readTagsWorker) || ActiveWorker(this.searchWorker);
        }

        private void ApplyGain_DoWork(object sender, DoWorkEventArgs e)
        {
            if (e.Argument is Mp3Folder folder)
            {
                folder.ApplyGainFolder(Executable, sender as BackgroundWorker);
                e.Result = folder;
            }
        }

        private void CalculateLength()
        {
            this.Length = this.Folders.Sum(x => x.Value.Length);
        }

        private BackgroundWorker CreateSearchWorker()
        {
            this.searchWorker = new BackgroundWorker();

            searchWorker.WorkerReportsProgress = true;
            searchWorker.WorkerSupportsCancellation = true;

            searchWorker.DoWork += SearchWorker_DoWork;
            searchWorker.ProgressChanged += SearchWorker_ProgressChanged;
            searchWorker.RunWorkerCompleted += SearchWorker_RunWorkerCompleted;

            return searchWorker;
        }

        private void ExecuteMP3Gain_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            

            if (e.UserState is FileProgress fileProg)
            {
                this.RaiseTaskProgressed(GetRunProgress(e.ProgressPercentage));

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

        private void ExecuteMP3Gain_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result is Mp3Folder folder)
            {
                string fileWord = WordWithEnding("file", folder.MP3Files);

                //Debug.WriteLine($"{folder.FolderName} is done. Gain used {folder.SuggestedGain} for {folder.MP3Files.Count} {fileWord}.");

                this.finished.Add(folder);

                var execute = new ExecuteMp3GainSync(Executable,
                                                 $"/o /s c",
                                                 folder.Files,
                                                 folder.FolderPath,
                                                 "GET MAX GAIN",
                                                 "Calculating Max Gain",
                                                 "done");

                execute.Execute();

                foreach (var file in folder.Files.Values)
                {
                    file.HasTags = false;
                    file.ExtractTags();
                }

                this.RaiseFolderFinished(this, folder);

                this.RunProgress = Helpers.GetProgress(this.finished.Count, this.Folders.Count);
                this.RaiseTaskProgressed(GetRunProgress(0));

                RunNextFolder();
            }

            if (this.FoldersLeft == 0)
            {
                this.Active = false;
                Debug.WriteLine("All folders are done.");
                this.RaiseAnalysisFinished();
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
                var mp3Folder = new Mp3Folder(folder);

                mp3Folder.FoundFile += Mp3Folder_FoundFile;
                mp3Folder.ChangedFile += Mp3Folder_ChangedFile;
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

        private void FindFilesUpdateTimer_Tick(object sender, EventArgs e)
        {
            this.RaiseSearchTimeElapsed();
        }

        private int GetNewFileCount(List<string> filePaths)
        {
            return filePaths.Except(this.foundFiles.Keys).ToList().Count;
        }

        private int GetRunProgress(int progressPercentage)
        {
            var progressSoFar = (progressPercentage * (1.0 / this.Folders.Count)) + this.RunProgress;

            return Convert.ToInt32(progressSoFar);
        }

        private void Mp3Folder_ChangedFile(object sender, Mp3File e)
        {
            this.RaiseChangedFile(sender, e);
        }

        private void Mp3Folder_FoundFile(object sender, Mp3File e)
        {
            this.RaiseFoundFile(sender, e);
        }
        private void ProcessFiles_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //if ((DateTime.Now - this.lastRefresh).TotalSeconds > .250)
            //{
            //    this.lastRefresh = DateTime.Now;
            //}
        }

        private void ProcessFiles_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result is Mp3Folder folder)
            {
                var execute = new ExecuteMp3GainSync(Executable,
                                                     $"/o /s c",
                                                     folder.Files,
                                                     folder.FolderPath,
                                                     "GET MAX GAIN",
                                                     "Calculating Max Gain",
                                                     "done");

                execute.Execute();

                foreach (var file in folder.Files.Values)
                {
                    file.HasTags = false;
                    file.ExtractTags();
                }

                string fileWord = WordWithEnding("file", folder.MP3Files);
                //Debug.WriteLine($"COMPLETE ANALYSIS FOR {folder.FolderName}. Gain used {folder.SuggestedGain} for {folder.MP3Files.Count} {fileWord}.");
                this.RaiseFolderFinished(this, folder);
                this.finished.Add(folder);

                RunNextFolder();
            }

            if (this.FoldersLeft == 0)
            {
                this.Active = false;
                Debug.WriteLine("All folders are done.");
                this.RaiseAnalysisFinished();
            }
        }

        private void RaiseAcivityUpdated(string message)
        {
            if (ActivityUpdated != null)
            {
                ActivityUpdated.Invoke(this, message);
            }
        }

        private void RaiseAnalysisFinished()
        {
            if (this.AnalysisFinished != null)
            {
                this.AnalysisFinished.Invoke(this, EventArgs.Empty);
            }
        }

        private void RaiseChangedFile(object sender, Mp3File e)
        {
            if (this.ChangedFile != null)
            {
                this.ChangedFile.Invoke(sender, e);
            }
        }

        private void RaiseFolderFinished(object sender, Mp3Folder folder)
        {
            this.FoldersLeft--;

            if (this.FolderFinished != null)
            {
                //this.FolderFinished.Invoke(sender, folder);
            }
        }

        private void RaiseFolderLoaded(Mp3Folder folder)
        {
            if (this.FolderLoaded != null)
            {
                this.FolderLoaded.Invoke(this, folder);
            }
        }

        private void RaiseFoundFile(object sender, Mp3File file)
        {
            if (this.FoundFile != null)
            {
                this.FoundFile.Invoke(sender, file);
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

        private void RaiseRowUpdated(int index)
        {
            if (this.RowUpdated != null)
            {
                this.RowUpdated.Invoke(this, index);
            }
        }

        private void RaiseSearchFinishedFolder()
        {
            if (this.SearchFinishedFolder != null)
            {
                this.SearchFinishedFolder.Invoke(this, null);
            }
        }

        private void RaiseSearchTimeElapsed()
        {
            if (SearchTimeElasped != null)
            {
                SearchTimeElasped.Invoke(this, this.ElaspedSearchTime);
            }
        }

        private void RaiseTagRead(Mp3File mP3GainFile)
        {
            if (this.TagRead != null)
            {
                this.TagRead.Invoke(this, mP3GainFile);
            }
        }

        private void RaiseTaskProgressed(int progress)
        {
            if (this.TaskProgressed != null)
            {
                this.TaskProgressed.Invoke(this, progress);
            }
        }

        private void ReadTagsWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (sender is BackgroundWorker worker)
            {
                var folders = this.AllFolders;
                this.filesDone = 0;
                var totalFiles = this.AllFiles.Count;

                worker.ReportProgress(0);

                foreach (var folder in folders)
                {
                    var execute = new ExecuteMp3GainSync(Executable,
                                                     $"/o /s c",
                                                     folder.Files,
                                                     folder.FolderPath,
                                                     "GET MAX GAIN",
                                                     "Calculating Max Gain",
                                                     "done");

                    execute.Execute();

                    foreach (var file in folder.Files.Values)
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
                }

                worker.ReportProgress(100);
            }
        }

        private void ReadTagsWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            var updateGui = this.readTagEventCheck.CheckTime(e.ProgressPercentage == 100);

            if (updateGui)
            {
                this.RaiseTaskProgressed(e.ProgressPercentage);
            }

            if (e.UserState is Mp3File file)
            {
                this.RaiseRowUpdated(file.SourceIndex);

                if (updateGui)
                {                  
                    this.RaiseTagRead(file);
                }
            }
            else if (e.UserState is string message)
            {
                this.RaiseAcivityUpdated(message);
            }
        }

        private void ReadTagsWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.CalculateLength();

            this.RaiseRefreshTable();
            this.RaiseAcivityUpdated("Finished.");
        }

        private void ResetAnalysis()
        {
            this.finished.Clear();
            this.RunProgress = 0;
            this.FoldersLeft = Folders.Values.Count;
        }

        private void RunFolderGroup(int cores = 2)
        {
            for (int i = 0; i < cores; i++)
            {
                RunNextFolder();
            }
        }

        private void RunNextFolder()
        {
            if (processStack.Count > 0)
            {
                var item = processStack.Pop();
                Debug.WriteLine($"POPPED[{processStack.Count}]: {((FolderWorker)item).Folder.FolderName}");
                item.Worker.RunWorkerAsync(item.Folder);
            }
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

                if (e.UserState is Mp3Folder folder)
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

        private void SearchWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (this.searchQeueu.Count > 0)
            {
                SearchFolders(this.searchQeueu);
            }

            this.RaiseRefreshTable();
            this.RaiseSearchFinishedFolder();
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

        private void UndoGain_DoWork(object sender, DoWorkEventArgs e)
        {
            if (e.Argument is Mp3Folder folder)
            {
                folder.UndoGain(Executable, sender as BackgroundWorker);
                e.Result = folder;
            }
        }
    }
}