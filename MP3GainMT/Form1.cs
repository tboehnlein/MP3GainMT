using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using WK.Libraries.BetterFolderBrowserNS;

namespace MP3GainMT
{
    public partial class Form1 : Form
    {
        private MP3GainRun run;
        private MP3GainSettings settings;
        private BindingSource source;
        private DateTime lastSort = DateTime.Now;
        private DateTime lastLabelRefresh = DateTime.Now;

        public Form1()
        {
            InitializeComponent();

            this.settings = new MP3GainSettings();
            this.source = new BindingSource();
            this.run = new MP3GainRun(@"C:\MP3Gain\mp3gain.exe");

            this.ReadSettings(run);

            this.run.FolderFinished += Run_FolderFinished;
            this.run.FoundFile += Run_FoundFile;
            this.run.ChangedFile += Run_ChangedFile;
            this.run.SearchFinishedFolder += Run_SearchFinishedFolder;
            this.run.RefreshTable += Run_RefreshTable;
            this.run.TaskProgressed += Run_TaskProgressed;
            this.run.RowUpdated += Run_RowUpdated;
            this.run.FolderLoaded += Run_FolderLoaded;
            this.run.TagRead += Run_TagRead;
            this.run.SearchTimeElasped += this.Run_SearchTimeElasped;
            this.run.ActivityUpdated += this.Run_ActivityFinished;
            this.run.AnalysisFinished += Run_AnalysisFinished;
            this.run.AskSearchQuestion += Run_AskSearchQuestion;
            this.folderPathTextBox.Text = run.ParentFolder;
            this.source.DataSource = this.run.DataSource;

            fileGridView.DataSource = source;

            this.ColumnTimer.Interval = 250;
            this.ColumnTimer.Tick += ColumnTimer_Tick;

            this.UpdateFileListLabel();
            this.CheckFolderPath();
        }

        private void ColumnTimer_Tick(object sender, EventArgs e)
        {
            this.ResizeAllColumns();

            this.ColumnTimer.Stop();
        }

        private void Run_ActivityFinished(object sender, string message)
        {
            this.activityLabel.Text = message;

            //ResizeAllColumns();
        }

        private void ReadSettings(MP3GainRun run)
        {
            this.Left = this.settings.LeftPosition;
            this.Top = this.settings.TopPosition;            
            this.Height = this.settings.HeightSize;
            this.Width = this.settings.WidthSize;

            run.ParentFolder = this.settings.ParentFolder;

            if (this.settings.TargetDb.CompareTo((double)this.targetDbNumeric.Value) > -1)
            {
                this.targetDbNumeric.Value = (decimal)this.settings.TargetDb;
            }
        }

        private void Run_TagRead(object sender, MP3GainFile e)
        {
            this.activityLabel.Text = $"Reading tag for {e.Folder}\\{e.FileName}";
            this.activityLabel.Refresh();
        }

        private void Run_FolderLoaded(object sender, MP3GainFolder e)
        {
            UpdateFileListLabel();
        }

        private void Run_TaskProgressed(object sender, int progress)
        {
            UpdateProgressBar(progress);
        }

        private void Run_RowUpdated(object sender, int index)
        {
            if (index > -1 && index < this.source.Count)
            {
                this.source.ResetItem(index);
            }
        }

        private void Run_AskSearchQuestion(object sender, string question)
        {
            if (question != string.Empty)
            {
            }
        }

        private void Run_AnalysisFinished(object sender, EventArgs e)
        {
            this.run.ResumeDataSource();
            this.fileGridView.ResumeLayout();
        }

        private void Run_SearchTimeElasped(object sender, TimeSpan e)
        {
            this.TickActivityLabel();
        }

        private void TickActivityLabel()
        {
            Helpers.UpdateProgressTick(this.SearchingActivity, this.activityLabel, 3);
        }

        

        private void UpdateProgressBar(int progress)
        {
            this.activityProgressBar.Value = progress;            

            if (progress == 100)
            {
                this.activityProgressBar.Enabled = false;
            }
            else
            {
                this.activityProgressBar.Enabled = true;
            }
        }

        public DateTime StartTime { get; private set; }
        public string SearchingActivity { get; private set; } = "[Searching folders]";
        public string LoadingFilesActivity { get; private set; } = "[Loading files]";

        private void UpdataDataGridView()
        {
            var index = this.fileGridView.FirstDisplayedScrollingRowIndex;

            this.fileGridView.SuspendLayout();
            run.RefreshDataSource();
            this.fileGridView.ResumeLayout();

            if (index >= 0)
            {
                this.fileGridView.FirstDisplayedScrollingRowIndex = index;
            }

            ResizeAllColumns();
        }

        private void BrowseButton_Click(object sender, EventArgs e)
        {
            var selectFolder = new BetterFolderBrowser();

            selectFolder.RootFolder = this.run.ParentFolder;

            var result = selectFolder.ShowDialog(this);

            if (result == DialogResult.OK)
            {
                this.folderPathTextBox.Text = selectFolder.SelectedFolder;
                this.settings.ParentFolder = selectFolder.SelectedFolder;

                CheckFolderPath();
            }
        }

        private bool CheckFolderPath()
        {
            if (Directory.Exists(this.folderPathTextBox.Text))
            {
                this.folderPathTextBox.BackColor = Color.White;
                this.run.ParentFolder = this.folderPathTextBox.Text;
                return true;
            }
            else
            {
                this.folderPathTextBox.BackColor = Color.Yellow;
                this.run.ParentFolder = string.Empty;
                return false;
            }
        }

        private void Run_RefreshTable(object sender, EventArgs e)
        {
            UpdataDataGridView();
            //this.activityLabel.Text = "Finished.";
        }

        private void Run_ChangedFile(object sender, MP3GainFile e)
        {
            //this.run.UpdateFile(e);
        }

        private void Run_SearchFinishedFolder(object sender, MP3GainFolder e)
        {
            run.RefreshDataSource(run.FolderFiles(e));
            SortTable();
        }

        private void RunButton_Click(object sender, EventArgs e)
        {
            this.StartTime = DateTime.Now;
            var parentFolder = this.folderPathTextBox.Text;

            if (Directory.Exists(parentFolder))
            {
                this.run.ApplyGain();
            }
        }

        private void Run_FoundFile(object sender, MP3GainFile e)
        {
            this.run.AddFile(e);
        }

        private void SortTable(bool force = false)
        {
            if (TimeCheck.Check(force, ref this.lastSort))
            {
                UpdateFileListLabel();
                this.UpdataDataGridView();
                //this.dataGridView1.Sort(this.dataGridView1.Columns["FullPath"], ListSortDirection.Ascending);
                this.Update();
                this.Refresh();
                UpdateFileListLabel();
            }
        }

        private void UpdateFileListLabel()
        {
            this.fileListLabel.Text = GetLoadedFilesLabel();
            this.fileListLabel.Refresh();
        }

        private string GetLoadedFilesLabel()
        {
            return $"Loaded Files [{GetFolderFileCountText()}]";
        }

        private string GetFolderFileCountText()
        {
            return $"Album Folder Count = {this.run.Folders.Count}, Song File Count = {this.run.FileCount}";
        }

        private void Run_FolderFinished(object sender, MP3GainFolder e)
        {
            Debug.WriteLine($"FOLDER {e.FolderName} FINISHED!");

            if (sender is MP3GainRun runner)
            {
                this.run.UpdateFolder(e);

                if (runner.FoldersLeft == 0)
                {
                    var finishTime = DateTime.Now;
                    var timeElapsed = finishTime - StartTime;
                    Debug.WriteLine($"COMPLETELY FINISHED!!!!!! TIME: {timeElapsed.Minutes}:{timeElapsed.Seconds}");
                    this.SortTable();
                }
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.run.CancelActivity();

            while (run.ActiveActivities)
            {
                this.run.CancelActivity();
                Application.DoEvents();
                Thread.Sleep(1000);                
            }

            WriteSettings();
        }

        private void WriteSettings()
        {
            this.settings.HeightSize = this.Height;
            this.settings.WidthSize = this.Width;
            this.settings.LeftPosition = this.Left;
            this.settings.TopPosition = this.Top;

            this.settings.ParentFolder = run.ParentFolder;

            this.settings.WriteSettingsFile();
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {
            if (CheckFolderPath())
            {
                this.activityLabel.Text = SearchingActivity;
                this.run.SearchFolders(this.run.ParentFolder);
            }
        }

        private void AnalyzeButton_Click(object sender, EventArgs e)
        {
            this.fileGridView.SuspendLayout();
            this.run.SuspendDataSource();

            this.StartTime = DateTime.Now;
            var parentFolder = this.folderPathTextBox.Text;

            if (Directory.Exists(parentFolder))
            {
                this.run.ProcessFiles();
            }
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            this.run.Clear();
            this.source.DataSource = this.run.DataSource;
        }

        private void ReadTagsButton_Click(object sender, EventArgs e)
        {
            if (CheckFolderPath())
            {
                this.run.ReadTags();
            }
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.run.CancelActivity();
        }

        private void TargetDbNumeric_ValueChanged(object sender, EventArgs e)
        {
            MP3GainRow.TargetDB = (double)this.targetDbNumeric.Value;

            var index = 0;

            foreach ( var row in this.run.DataSource)
            {
                this.source.ResetItem(index);
                this.fileGridView.Update();
                index++;
            }

            this.fileGridView.Refresh();

        }

        private void ClipOnlyCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (this.clipOnlyCheckBox.Checked)
            {
                this.run.DataSource.ApplyFilter(delegate (MP3GainRow row) { return row.TrackClipping || row.AlbumClipping; });
            }
            else
            {
                this.run.DataSource.RemoveFilter();
            }
        }

        private void ResizeAllColumns()
        {
            ResizeColumnWidth(this.Folder.Name);
            ResizeColumnWidth(this.AlbumArtist.Name);
            ResizeColumnWidth(this.Album.Name);
            ResizeColumnWidth(this.Artist.Name);
        }

        /// <summary>
        /// Resizes a column to fit the width of the displayed cells and make that the minimum width.
        /// </summary>
        /// <param name="name">Name of the column.</param>
        private void ResizeColumnWidth(string name)
        {
            var artistColumn = this.fileGridView.Columns[name];
            this.fileGridView.AutoResizeColumn(artistColumn.Index, DataGridViewAutoSizeColumnMode.DisplayedCells);
            var artistMinWidth = artistColumn.GetPreferredWidth(DataGridViewAutoSizeColumnMode.DisplayedCells, true);
            if (artistColumn.MinimumWidth < artistMinWidth)
            {
                artistColumn.MinimumWidth = artistMinWidth;
            }
        }

        private System.Windows.Forms.Timer ColumnTimer = new System.Windows.Forms.Timer();

        private void FileGridView_Scroll(object sender, ScrollEventArgs e)
        {
            this.ColumnTimer.Stop();
            this.ColumnTimer.Start();
        }

        private void UndoButton_Click(object sender, EventArgs e)
        {
            this.run.UndoGain();
        }
    }
}