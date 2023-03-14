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

        
        private MP3GainRun run;        
        private BindingSource source;
        private DateTime lastSort = DateTime.Now;

        public Form1()
        {
            InitializeComponent();

            this.source = new BindingSource();
            this.run = new MP3GainRun(@"C:\MP3Gain\mp3gain.exe");

            this.run.FolderFinished += Run_FolderFinished;
            this.run.FoundFile += Run_FoundFile;
            this.run.ChangedFile += Run_ChangedFile;
            this.run.SearchFinishedFolder += Run_SearchFinishedFolder;
            this.run.RefreshTable += Run_RefreshTable;

            this.source.DataSource = this.run.DataSource;


            dataGridView1.DataSource = source;
        }

        public DateTime StartTime { get; private set; }

        private void RefreshRows()
        {
            run.RefreshDataSource();
        }

        

        private void BrowseButton_Click(object sender, EventArgs e)
        {
            var selectFolder = new BetterFolderBrowser();

            selectFolder.RootFolder = this.run.ParentFolder;

            var result = selectFolder.ShowDialog(this);

            if (result == DialogResult.OK)
            {
                this.run.Clear();
                this.folderPathTextBox.Text = selectFolder.SelectedFolder;
                var parentFolder = this.folderPathTextBox.Text;

                this.run.SearchFolders(parentFolder);
            }

        }

        private void Run_RefreshTable(object sender, EventArgs e)
        {
            run.RefreshDataSource();
            this.SortTable(true);
            this.Update();
            this.Refresh();
        }

        private void Run_ChangedFile(object sender, MP3GainFile e)
        {
            this.run.UpdateFile(e);
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
                this.run.Run();
            }
        }

        private void Run_FoundFile(object sender, MP3GainFile e)
        {
            this.run.AddFile(e);
        }

        private void SortTable(bool force = false)
        {

            if ((DateTime.Now - this.lastSort).TotalSeconds > .250  && force)
            {
                this.RefreshRows();
                this.dataGridView1.Sort(this.dataGridView1.Columns["FullPath"], ListSortDirection.Ascending);
                this.Update();
                this.Refresh();
                this.lastSort = DateTime.Now;
            }
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
    }
}
