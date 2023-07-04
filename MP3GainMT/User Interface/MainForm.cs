using MP3GainMT.MP3Gain;
using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using WK.Libraries.BetterFolderBrowserNS;

namespace MP3GainMT
{
    public partial class MainForm : Form
    {
        private System.Windows.Forms.Timer ColumnTimer = new System.Windows.Forms.Timer();
        private DateTime lastLabelRefresh = DateTime.Now;
        private DateTime lastSort = DateTime.Now;
        private Mp3GainManager run;
        private MP3GainSettings settings;
        private BindingSource source;

        public MainForm()
        {
            InitializeComponent();

            this.settings = new MP3GainSettings();
            this.source = new BindingSource();
            this.run = new Mp3GainManager(@"C:\MP3Gain\mp3gain.exe");

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

        public string LoadingFilesActivity { get; private set; } = "[Loading files]";

        public string SearchingActivity { get; private set; } = "[Searching folders]";

        public DateTime StartTime { get; private set; }

        private void AnalyzeButton_Click(object sender, EventArgs e)
        {
            this.fileGridView.SuspendLayout();
            this.run.SuspendDataSource();

            this.StartTime = DateTime.Now;
            var parentFolder = this.folderPathTextBox.Text;

            if (Directory.Exists(parentFolder))
            {
                this.run.AnalyzeFiles();
            }
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

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.run.CancelActivity();
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

        private void ClearButton_Click(object sender, EventArgs e)
        {
            this.run.Clear();
            this.ClearColumnWidths();
            this.source.DataSource = this.run.DataSource;
        }

        private void ClearColumnWidths()
        {
            ResetColumnWidth(this.Folder.Name);
            ResetColumnWidth(this.AlbumArtist.Name);
            ResetColumnWidth(this.Album.Name);
            ResetColumnWidth(this.Artist.Name);
        }

        private void ResetColumnWidth(string name)
        {
            var gridViewColumn = this.fileGridView.Columns[name];
            this.fileGridView.AutoResizeColumn(gridViewColumn.Index, DataGridViewAutoSizeColumnMode.DisplayedCells);
            var artistMinWidth = gridViewColumn.GetPreferredWidth(DataGridViewAutoSizeColumnMode.DisplayedCells, true);
            gridViewColumn.MinimumWidth = artistMinWidth;
        }

        private void ClipOnlyCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            DefineFilters();
        }

        private void ColumnTimer_Tick(object sender, EventArgs e)
        {
            this.ResizeAllColumns();

            this.ColumnTimer.Stop();
        }

        private void FileGridView_Scroll(object sender, ScrollEventArgs e)
        {
            this.ColumnTimer.Stop();
            this.ColumnTimer.Start();
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

        private string GetFolderFileCountText()
        {
            return $"Album Folder Count = {this.run.Folders.Count}, Song File Count = {this.run.FileCount}";
        }

        private string GetLoadedFilesLabel()
        {
            return $"Loaded Files [{GetFolderFileCountText()}]";
        }

        private void ReadSettings(Mp3GainManager run)
        {
            this.Left = this.settings.LeftPosition;
            this.Top = this.settings.TopPosition;
            this.Height = this.settings.HeightSize;
            this.Width = this.settings.WidthSize;
            this.andRadioButton.Checked = this.settings.UseAnd;
            this.orRadioButton.Checked = !this.settings.UseAnd;

            run.ParentFolder = this.settings.ParentFolder;

            if (this.settings.TargetDb.CompareTo((double)this.targetDbNumeric.Value) > -1)
            {
                this.targetDbNumeric.Value = (decimal)this.settings.TargetDb;
            }

            this.fileGridView.Columns["FullPath"].Width = this.settings.PathWidth;
        }

        private void ReadTagsButton_Click(object sender, EventArgs e)
        {
            if (CheckFolderPath())
            {
                this.run.ReadTags();
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

        private void Run_ActivityFinished(object sender, string message)
        {
            this.activityLabel.Text = message;

            //ResizeAllColumns();
        }

        private void Run_AnalysisFinished(object sender, EventArgs e)
        {
            this.run.ResumeDataSource();
            this.fileGridView.ResumeLayout();
        }

        private void Run_AskSearchQuestion(object sender, string question)
        {
            if (question != string.Empty)
            {
            }
        }

        private void Run_ChangedFile(object sender, Mp3File e)
        {
            //this.run.UpdateFile(e);
        }

        private void Run_FolderFinished(object sender, Mp3Folder e)
        {
            //Debug.WriteLine($"FOLDER {e.FolderName} FINISHED!");

            if (sender is Mp3GainManager runner)
            {
                this.run.UpdateFolder(e);

                if (runner.FoldersLeft == 0)
                {
                    var finishTime = DateTime.Now;
                    var timeElapsed = finishTime - StartTime;
                    //Debug.WriteLine($"COMPLETELY FINISHED!!!!!! TIME: {timeElapsed.Minutes}:{timeElapsed.Seconds}");
                    this.SortTable();
                }
            }
        }

        private void Run_FolderLoaded(object sender, Mp3Folder e)
        {
            UpdateFileListLabel();
        }

        private void Run_FoundFile(object sender, Mp3File e)
        {
            this.run.AddFile(e);
        }

        private void Run_RefreshTable(object sender, EventArgs e)
        {
            UpdataDataGridView();
            //this.activityLabel.Text = "Finished.";
        }

        private void Run_RowUpdated(object sender, int index)
        {
            if (index > -1 && index < this.source.Count)
            {
                this.source.ResetItem(index);
            }
        }

        private void Run_SearchFinishedFolder(object sender, Mp3Folder e)
        {
            run.RefreshDataSource(run.FolderFiles(e));
            SortTable();
        }

        private void Run_SearchTimeElasped(object sender, TimeSpan e)
        {
            this.TickActivityLabel();
        }

        private void Run_TagRead(object sender, Mp3File e)
        {
            this.activityLabel.Text = $"Reading tag for {e.Folder}\\{e.FileName}";
            this.activityLabel.Refresh();
        }

        private void Run_TaskProgressed(object sender, int progress)
        {
            UpdateProgressBar(progress);
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

        private void SearchButton_Click(object sender, EventArgs e)
        {
            if (CheckFolderPath())
            {
                this.activityLabel.Text = SearchingActivity;
                this.run.SearchFolders(this.run.ParentFolder);
            }
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

        private void TargetDbNumeric_ValueChanged(object sender, EventArgs e)
        {
            MP3GainRow.TargetDB = (double)this.targetDbNumeric.Value;

            var index = 0;

            foreach (var row in this.run.DataSource)
            {
                this.source.ResetItem(index);
                this.fileGridView.Update();
                index++;
            }

            this.fileGridView.Refresh();
        }

        private void TickActivityLabel()
        {
            Helpers.UpdateProgressTick(this.SearchingActivity, this.activityLabel, 3);
        }

        private void UndoButton_Click(object sender, EventArgs e)
        {
            this.run.UndoGain();
        }

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

        private void UpdateFileListLabel()
        {
            this.fileListLabel.Text = GetLoadedFilesLabel();
            this.fileListLabel.Refresh();
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

            //Debug.WriteLine($"TASK PROGRESS: {progress}");
        }

        private void WriteSettings()
        {
            this.settings.HeightSize = this.Height;
            this.settings.WidthSize = this.Width;
            this.settings.LeftPosition = this.Left;
            this.settings.TopPosition = this.Top;
            this.settings.PathWidth = this.fileGridView.Columns["FullPath"].Width;
            this.settings.UseAnd = this.andRadioButton.Checked;

            this.settings.ParentFolder = run.ParentFolder;

            this.settings.WriteSettingsFile();
        }

        private void FileGridView_CellToolTipTextNeeded(object sender, DataGridViewCellToolTipTextNeededEventArgs e)
        {
            var folderIndex = this.fileGridView.Columns["Folder"].Index;
            var pathIndex = this.fileGridView.Columns["FullPath"].Index;
            int fileIndex = this.fileGridView.Columns["FileName"].Index;

            if ((e.ColumnIndex == folderIndex || e.ColumnIndex == fileIndex) && e.RowIndex >= 0)
            {
                var cell = this.fileGridView.Rows[e.RowIndex].Cells[pathIndex];
                e.ToolTipText = cell.Value.ToString();
            }
        }

        public bool CheckAlbumClipOnly(MP3GainRow row, bool apply)
        {
            if (!apply) { return true; }

            return row.AlbumClipping; ;
        }

        public bool CheckTrackClipOnly(MP3GainRow row, bool apply)
        {
            if (!apply) { return true; }

            return row.TrackClipping;
        }

        public bool CheckThreshOnly(MP3GainRow row, bool apply, double threshold)
        {
            if (!apply) { return true; }

            return row.TrackDB > threshold;
        }

        private void DefineFilters()
        {
            this.run.DataSource.RemoveFilter();

            var threshold = Convert.ToDouble(this.threshNumeric.Value);
            var useThresh = this.threshCheckBox.Checked;
            var useTrackClipping = this.clipOnlyTrackCheckBox.Checked;
            var useAlbumClipping = this.clipOnlyAlbumCheckBox.Checked;
            var useClipping = this.clipOnlyCheckBox.Checked;
            var searchText = this.searchTextBox.Text;
            var useText = searchText.Length > 0;
            var useAnd = this.andRadioButton.Checked;

            if (this.threshCheckBox.Checked || this.clipOnlyTrackCheckBox.Checked || this.clipOnlyAlbumCheckBox.Checked || this.clipOnlyCheckBox.Checked || useText)
            {
                this.run.DataSource.ApplyFilter(delegate (MP3GainRow row) { return CheckClipOnly(row, useClipping) && CheckTrackClipOnly(row, useTrackClipping) && CheckAlbumClipOnly(row, useAlbumClipping) && CheckThreshOnly(row, useThresh, threshold) && SearchFilePath(row, searchText, useAnd); });
            }
        }

        private bool SearchFilePath(MP3GainRow row, string searchText, bool useAnd)
        {
            if (searchText.Length == 0) { return true; }

            var words = searchText.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            var found = useAnd;

            foreach (var word in words)
            {
                var search = word.Trim();

                if (search.Length > 0)
                {
                    found = row.FullPath.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0;

                    if (useAnd)
                    {
                        if (!found)
                        {
                            break;
                        }
                    }
                    else
                    {
                        if (found)
                        {
                            break;
                        }
                    }
                }
            }

            return found;
        }

        private bool CheckClipOnly(MP3GainRow row, bool apply)
        {
            if (!apply) { return true; }

            return row.Clipping;
        }

        private void ThreshCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            DefineFilters();
        }

        private void ThreshDbNumeric_ValueChanged(object sender, EventArgs e)
        {
            DefineFilters();
        }

        private void FileGridView_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var isAltColor = Convert.ToBoolean(fileGridView.Rows[e.RowIndex].Cells[14].Value);
                var color = Color.White;

                if (e.RowIndex % 2 == 0)
                {
                    color = isAltColor ? Color.FromArgb(225, 225, 255) : Color.FromArgb(245, 245, 245);
                }
                else
                {
                    color = isAltColor ? Color.FromArgb(240, 240, 255) : Color.White;
                }

                fileGridView.Rows[e.RowIndex].DefaultCellStyle.BackColor = color;
            }
        }

        private void SearchTextBox_TextChanged(object sender, EventArgs e)
        {
            DefineFilters();
        }

        private void RemoveButton_Click(object sender, EventArgs e)
        {
            searchTextBox.Text = string.Empty;
        }

        private void SearchRadio_CheckChanged(object sender, EventArgs e)
        {
            DefineFilters();
        }
    }
}