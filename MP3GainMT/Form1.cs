using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinFormMP3Gain;
using WK.Libraries.BetterFolderBrowserNS;

namespace MP3GainMT
{
    public partial class Form1 : Form
    {

        private Dictionary<string, MP3GainFile> foundFiles = new Dictionary<string, MP3GainFile>();
        private MP3GainRun run;
        private SortableBindingList<MP3GainRow> rows = new SortableBindingList<MP3GainRow>();
        private BindingSource source;

        public Form1()
        {
            InitializeComponent();

            this.source = new BindingSource();

            this.source.DataSource = this.rows;


            dataGridView1.DataSource = source;
        }

        public DateTime StartTime { get; private set; }

        private void RefreshRows()
        {
            foreach (var file in foundFiles.Values)
            {
                UpdateRow(file);
            }
        }

        private void UpdateRow(MP3GainFile file)
        {
            if (this.run.Folders.ContainsKey(file.FolderPath))
            {
                var match = rows.Select(x => x).Where(x => x.FullPath == file.FilePath);

                if (!match.Any())
                {
                    rows.Add(new MP3GainRow(file, this.run.Folders[file.FolderPath]));
                }
            }
        }

        private void BrowseButton_Click(object sender, EventArgs e)
        {
            var selectFolder = new BetterFolderBrowser();

            var result = selectFolder.ShowDialog(this);

            if (result == DialogResult.OK)
            {
                this.foundFiles.Clear();
                this.rows.Clear();
                this.folderPathTextBox.Text = selectFolder.SelectedFolder;
                var parentFolder = this.folderPathTextBox.Text;

                this.run = new MP3GainRun(@"C:\MP3Gain\mp3gain.exe", parentFolder);

                this.run.FolderFinished += Run_FolderFinished;
                this.run.FoundFile += Run_FoundFile;
                this.run.ChangedFile += Run_ChangedFile;
                this.run.SearchFinished += Run_SearchFinished;
                this.run.RefreshTable += Run_RefreshTable;

                this.run.SearchFolders();
            }

        }

        private void Run_RefreshTable(object sender, EventArgs e)
        {
            this.Update();
            this.Refresh();
        }

        private void Run_ChangedFile(object sender, MP3GainFile e)
        {
            UpdateRow(e);
        }

        private void Run_SearchFinished(object sender, MP3GainFolder e)
        {
            SortTable();
        }

        private void RunButton_Click(object sender, EventArgs e)
        {
            this.StartTime = DateTime.Now;  
            var parentFolder = this.folderPathTextBox.Text;

            if (Directory.Exists(parentFolder))
            {
                this.run.Run();
            }
        }

        private void Run_FoundFile(object sender, MP3GainFile e)
        {
            if (!this.foundFiles.ContainsKey(e.FilePath))
            {
                this.foundFiles.Add(e.FilePath, e);
            }
            else
            {
                UpdateRow(e);
            }
        }

        private void SortTable()
        {
            this.RefreshRows();
            this.dataGridView1.Sort(this.dataGridView1.Columns["FullPath"], ListSortDirection.Ascending);
            this.Update();
            this.Refresh();
        }

        private void Run_FolderFinished(object sender, MP3GainFolder e)
        {
            Debug.WriteLine($"FOLDER {e.FolderName} FINISHED!");

            if (sender is MP3GainRun runner)
            {
                if (this.foundFiles.Select( x => x).Where( x => x.Value.FolderPath == e.FolderPath ).Any())
                {
                    this.RefreshRows();
                }

                if (runner.FoldersLeft == 0)
                {
                    var finishTime = DateTime.Now;
                    var timeElapsed = finishTime - StartTime;
                    Debug.WriteLine($"COMPLETELY FINISHED!!!!!! TIME: {timeElapsed.Minutes}:{timeElapsed.Seconds}");
                    this.SortTable();
                }
            }
        }
    }
}
