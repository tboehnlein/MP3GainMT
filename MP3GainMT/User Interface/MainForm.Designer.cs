﻿using System.Windows.Forms;
using MP3GainMT.User_Interface;

namespace MP3GainMT
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.browseButton = new System.Windows.Forms.Button();
            this.folderPathTextBox = new System.Windows.Forms.TextBox();
            this.runButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.fileListLabel = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.clearButton = new System.Windows.Forms.Button();
            this.searchButton = new System.Windows.Forms.Button();
            this.activityProgressBar = new System.Windows.Forms.ProgressBar();
            this.readTagsButton = new System.Windows.Forms.Button();
            this.activityLabel = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.activityPanel = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.cancelButton = new System.Windows.Forms.Button();
            this.targetDbNumeric = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.clipOnlyTrackCheckBox = new System.Windows.Forms.CheckBox();
            this.filterGroupBox = new System.Windows.Forms.GroupBox();
            this.threshNumeric = new System.Windows.Forms.NumericUpDown();
            this.threshCheckBox = new System.Windows.Forms.CheckBox();
            this.clipOnlyAlbumCheckBox = new System.Windows.Forms.CheckBox();
            this.threshLabel = new System.Windows.Forms.Label();
            this.undoButton = new System.Windows.Forms.Button();
            this.clipOnlyCheckBox = new System.Windows.Forms.CheckBox();
            this.fileGridView = new MP3GainMT.User_Interface.DataGridViewBuffered();
            this.FullPath = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.AlbumArtist = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Updated = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Folder = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FileName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Album = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Artist = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Progress = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TrackDB = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Clipping = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.TrackFinal = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TrackClip = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.AlbumDB = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AlbumGain = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AlbumClip = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.ErrorMessage = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.activityPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.targetDbNumeric)).BeginInit();
            this.filterGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.threshNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fileGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // browseButton
            // 
            this.browseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.browseButton.Location = new System.Drawing.Point(371, 55);
            this.browseButton.Name = "browseButton";
            this.browseButton.Size = new System.Drawing.Size(64, 28);
            this.browseButton.TabIndex = 2;
            this.browseButton.Text = "&Browse";
            this.browseButton.UseVisualStyleBackColor = true;
            this.browseButton.Click += new System.EventHandler(this.BrowseButton_Click);
            // 
            // folderPathTextBox
            // 
            this.folderPathTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.folderPathTextBox.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.folderPathTextBox.Location = new System.Drawing.Point(19, 59);
            this.folderPathTextBox.Name = "folderPathTextBox";
            this.folderPathTextBox.ReadOnly = true;
            this.folderPathTextBox.Size = new System.Drawing.Size(346, 20);
            this.folderPathTextBox.TabIndex = 1;
            // 
            // runButton
            // 
            this.runButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.runButton.Location = new System.Drawing.Point(780, 54);
            this.runButton.Name = "runButton";
            this.runButton.Size = new System.Drawing.Size(87, 28);
            this.runButton.TabIndex = 6;
            this.runButton.Text = "Apply &Gain";
            this.runButton.UseVisualStyleBackColor = true;
            this.runButton.Click += new System.EventHandler(this.RunButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 43);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(151, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Parent Folder of Album Folders";
            // 
            // fileListLabel
            // 
            this.fileListLabel.AutoSize = true;
            this.fileListLabel.Location = new System.Drawing.Point(16, 146);
            this.fileListLabel.Name = "fileListLabel";
            this.fileListLabel.Size = new System.Drawing.Size(271, 13);
            this.fileListLabel.TabIndex = 8;
            this.fileListLabel.Text = "Loaded Files [Album Folder Count = %s, File Count = %s]";
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(712, 55);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(62, 28);
            this.button1.TabIndex = 5;
            this.button1.Text = "&Analyze";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.AnalyzeButton_Click);
            // 
            // clearButton
            // 
            this.clearButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.clearButton.Location = new System.Drawing.Point(873, 54);
            this.clearButton.Name = "clearButton";
            this.clearButton.Size = new System.Drawing.Size(56, 28);
            this.clearButton.TabIndex = 7;
            this.clearButton.Text = "&Clear";
            this.clearButton.UseVisualStyleBackColor = true;
            this.clearButton.Click += new System.EventHandler(this.ClearButton_Click);
            // 
            // searchButton
            // 
            this.searchButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.searchButton.Location = new System.Drawing.Point(441, 55);
            this.searchButton.Name = "searchButton";
            this.searchButton.Size = new System.Drawing.Size(87, 28);
            this.searchButton.TabIndex = 3;
            this.searchButton.Text = "Add &Files";
            this.searchButton.UseVisualStyleBackColor = true;
            this.searchButton.Click += new System.EventHandler(this.SearchButton_Click);
            // 
            // activityProgressBar
            // 
            this.activityProgressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.activityProgressBar.Location = new System.Drawing.Point(803, 5);
            this.activityProgressBar.Name = "activityProgressBar";
            this.activityProgressBar.Size = new System.Drawing.Size(277, 19);
            this.activityProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.activityProgressBar.TabIndex = 11;
            // 
            // readTagsButton
            // 
            this.readTagsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.readTagsButton.Location = new System.Drawing.Point(619, 55);
            this.readTagsButton.Name = "readTagsButton";
            this.readTagsButton.Size = new System.Drawing.Size(87, 28);
            this.readTagsButton.TabIndex = 4;
            this.readTagsButton.Text = "&Read Tags";
            this.readTagsButton.UseVisualStyleBackColor = true;
            this.readTagsButton.Click += new System.EventHandler(this.ReadTagsButton_Click);
            // 
            // activityLabel
            // 
            this.activityLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.activityLabel.AutoEllipsis = true;
            this.activityLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.activityLabel.ForeColor = System.Drawing.Color.White;
            this.activityLabel.Location = new System.Drawing.Point(55, 7);
            this.activityLabel.Margin = new System.Windows.Forms.Padding(3, 0, 20, 0);
            this.activityLabel.Name = "activityLabel";
            this.activityLabel.Size = new System.Drawing.Size(665, 17);
            this.activityLabel.TabIndex = 10;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(4, 7);
            this.label2.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "Activity: ";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // activityPanel
            // 
            this.activityPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.activityPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.activityPanel.Controls.Add(this.label3);
            this.activityPanel.Controls.Add(this.activityProgressBar);
            this.activityPanel.Controls.Add(this.activityLabel);
            this.activityPanel.Controls.Add(this.label2);
            this.activityPanel.Location = new System.Drawing.Point(0, 0);
            this.activityPanel.Name = "activityPanel";
            this.activityPanel.Size = new System.Drawing.Size(1085, 29);
            this.activityPanel.TabIndex = 12;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(721, 7);
            this.label3.Margin = new System.Windows.Forms.Padding(20, 0, 0, 0);
            this.label3.Name = "label3";
            this.label3.Padding = new System.Windows.Forms.Padding(20, 0, 0, 0);
            this.label3.Size = new System.Drawing.Size(80, 13);
            this.label3.TabIndex = 12;
            this.label3.Text = "Progress:";
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.Location = new System.Drawing.Point(935, 55);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(55, 28);
            this.cancelButton.TabIndex = 7;
            this.cancelButton.Text = "Ca&ncel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // targetDbNumeric
            // 
            this.targetDbNumeric.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.targetDbNumeric.DecimalPlaces = 1;
            this.targetDbNumeric.Location = new System.Drawing.Point(544, 60);
            this.targetDbNumeric.Maximum = new decimal(new int[] {
            1059,
            0,
            0,
            65536});
            this.targetDbNumeric.Minimum = new decimal(new int[] {
            757,
            0,
            0,
            65536});
            this.targetDbNumeric.Name = "targetDbNumeric";
            this.targetDbNumeric.Size = new System.Drawing.Size(50, 20);
            this.targetDbNumeric.TabIndex = 13;
            this.targetDbNumeric.Value = new decimal(new int[] {
            89,
            0,
            0,
            0});
            this.targetDbNumeric.ValueChanged += new System.EventHandler(this.TargetDbNumeric_ValueChanged);
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(546, 43);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(38, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Target";
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(597, 63);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(20, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "dB";
            // 
            // clipOnlyTrackCheckBox
            // 
            this.clipOnlyTrackCheckBox.AutoSize = true;
            this.clipOnlyTrackCheckBox.Location = new System.Drawing.Point(73, 19);
            this.clipOnlyTrackCheckBox.Name = "clipOnlyTrackCheckBox";
            this.clipOnlyTrackCheckBox.Size = new System.Drawing.Size(74, 17);
            this.clipOnlyTrackCheckBox.TabIndex = 14;
            this.clipOnlyTrackCheckBox.Text = "Track Clip";
            this.clipOnlyTrackCheckBox.UseVisualStyleBackColor = true;
            this.clipOnlyTrackCheckBox.CheckedChanged += new System.EventHandler(this.ClipOnlyCheckBox_CheckedChanged);
            // 
            // filterGroupBox
            // 
            this.filterGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.filterGroupBox.Controls.Add(this.clipOnlyCheckBox);
            this.filterGroupBox.Controls.Add(this.threshNumeric);
            this.filterGroupBox.Controls.Add(this.threshCheckBox);
            this.filterGroupBox.Controls.Add(this.clipOnlyAlbumCheckBox);
            this.filterGroupBox.Controls.Add(this.clipOnlyTrackCheckBox);
            this.filterGroupBox.Controls.Add(this.threshLabel);
            this.filterGroupBox.Location = new System.Drawing.Point(619, 89);
            this.filterGroupBox.Name = "filterGroupBox";
            this.filterGroupBox.Size = new System.Drawing.Size(454, 42);
            this.filterGroupBox.TabIndex = 15;
            this.filterGroupBox.TabStop = false;
            this.filterGroupBox.Text = "Filter Options";
            // 
            // threshNumeric
            // 
            this.threshNumeric.DecimalPlaces = 1;
            this.threshNumeric.Location = new System.Drawing.Point(306, 16);
            this.threshNumeric.Maximum = new decimal(new int[] {
            1059,
            0,
            0,
            65536});
            this.threshNumeric.Minimum = new decimal(new int[] {
            757,
            0,
            0,
            65536});
            this.threshNumeric.Name = "threshNumeric";
            this.threshNumeric.Size = new System.Drawing.Size(50, 20);
            this.threshNumeric.TabIndex = 13;
            this.threshNumeric.Value = new decimal(new int[] {
            89,
            0,
            0,
            0});
            this.threshNumeric.ValueChanged += new System.EventHandler(this.ThreshDbNumeric_ValueChanged);
            // 
            // threshCheckBox
            // 
            this.threshCheckBox.AutoSize = true;
            this.threshCheckBox.BackColor = System.Drawing.Color.Transparent;
            this.threshCheckBox.Location = new System.Drawing.Point(276, 19);
            this.threshCheckBox.Name = "threshCheckBox";
            this.threshCheckBox.Size = new System.Drawing.Size(32, 17);
            this.threshCheckBox.TabIndex = 14;
            this.threshCheckBox.Text = ">";
            this.threshCheckBox.UseVisualStyleBackColor = false;
            this.threshCheckBox.CheckedChanged += new System.EventHandler(this.ThreshCheckBox_CheckedChanged);
            // 
            // clipOnlyAlbumCheckBox
            // 
            this.clipOnlyAlbumCheckBox.AutoSize = true;
            this.clipOnlyAlbumCheckBox.Location = new System.Drawing.Point(173, 19);
            this.clipOnlyAlbumCheckBox.Name = "clipOnlyAlbumCheckBox";
            this.clipOnlyAlbumCheckBox.Size = new System.Drawing.Size(75, 17);
            this.clipOnlyAlbumCheckBox.TabIndex = 14;
            this.clipOnlyAlbumCheckBox.Text = "Album Clip";
            this.clipOnlyAlbumCheckBox.UseVisualStyleBackColor = true;
            this.clipOnlyAlbumCheckBox.CheckedChanged += new System.EventHandler(this.ClipOnlyCheckBox_CheckedChanged);
            // 
            // threshLabel
            // 
            this.threshLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.threshLabel.AutoSize = true;
            this.threshLabel.Location = new System.Drawing.Point(304, 19);
            this.threshLabel.Name = "threshLabel";
            this.threshLabel.Size = new System.Drawing.Size(20, 13);
            this.threshLabel.TabIndex = 0;
            this.threshLabel.Text = "dB";
            // 
            // undoButton
            // 
            this.undoButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.undoButton.Location = new System.Drawing.Point(996, 55);
            this.undoButton.Name = "undoButton";
            this.undoButton.Size = new System.Drawing.Size(56, 28);
            this.undoButton.TabIndex = 7;
            this.undoButton.Text = "&Undo";
            this.undoButton.UseVisualStyleBackColor = true;
            this.undoButton.Click += new System.EventHandler(this.UndoButton_Click);
            // 
            // clipOnlyCheckBox
            // 
            this.clipOnlyCheckBox.AutoSize = true;
            this.clipOnlyCheckBox.Location = new System.Drawing.Point(13, 19);
            this.clipOnlyCheckBox.Name = "clipOnlyCheckBox";
            this.clipOnlyCheckBox.Size = new System.Drawing.Size(43, 17);
            this.clipOnlyCheckBox.TabIndex = 15;
            this.clipOnlyCheckBox.Text = "Clip";
            this.clipOnlyCheckBox.UseVisualStyleBackColor = true;
            this.clipOnlyCheckBox.CheckedChanged += new System.EventHandler(this.ClipOnlyCheckBox_CheckedChanged);
            // 
            // fileGridView
            // 
            this.fileGridView.AllowUserToAddRows = false;
            this.fileGridView.AllowUserToDeleteRows = false;
            this.fileGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fileGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.fileGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.FullPath,
            this.AlbumArtist,
            this.Updated,
            this.Folder,
            this.FileName,
            this.Album,
            this.Artist,
            this.Progress,
            this.TrackDB,
            this.Clipping,
            this.TrackFinal,
            this.TrackClip,
            this.AlbumDB,
            this.AlbumGain,
            this.AlbumClip,
            this.ErrorMessage});
            this.fileGridView.Location = new System.Drawing.Point(12, 162);
            this.fileGridView.MultiSelect = false;
            this.fileGridView.Name = "fileGridView";
            this.fileGridView.ReadOnly = true;
            this.fileGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.fileGridView.Size = new System.Drawing.Size(1061, 446);
            this.fileGridView.TabIndex = 9;
            this.fileGridView.CellToolTipTextNeeded += new System.Windows.Forms.DataGridViewCellToolTipTextNeededEventHandler(this.FileGridView_CellToolTipTextNeeded);
            this.fileGridView.Scroll += new System.Windows.Forms.ScrollEventHandler(this.FileGridView_Scroll);
            // 
            // FullPath
            // 
            this.FullPath.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.FullPath.DataPropertyName = "FullPath";
            this.FullPath.HeaderText = "Full Path";
            this.FullPath.Name = "FullPath";
            this.FullPath.ReadOnly = true;
            this.FullPath.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.FullPath.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.FullPath.Visible = false;
            // 
            // AlbumArtist
            // 
            this.AlbumArtist.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.AlbumArtist.DataPropertyName = "AlbumArtist";
            this.AlbumArtist.HeaderText = "Album Artist";
            this.AlbumArtist.Name = "AlbumArtist";
            this.AlbumArtist.ReadOnly = true;
            this.AlbumArtist.Visible = false;
            // 
            // Updated
            // 
            this.Updated.DataPropertyName = "Updated";
            this.Updated.HeaderText = "Updated";
            this.Updated.Name = "Updated";
            this.Updated.ReadOnly = true;
            this.Updated.Visible = false;
            // 
            // Folder
            // 
            this.Folder.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Folder.DataPropertyName = "Folder";
            this.Folder.HeaderText = "Folder";
            this.Folder.Name = "Folder";
            this.Folder.ReadOnly = true;
            // 
            // FileName
            // 
            this.FileName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.FileName.DataPropertyName = "FileName";
            this.FileName.HeaderText = "File Name";
            this.FileName.Name = "FileName";
            this.FileName.ReadOnly = true;
            // 
            // Album
            // 
            this.Album.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.Album.DataPropertyName = "Album";
            this.Album.HeaderText = "Album";
            this.Album.Name = "Album";
            this.Album.ReadOnly = true;
            this.Album.Visible = false;
            // 
            // Artist
            // 
            this.Artist.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.Artist.DataPropertyName = "Artist";
            this.Artist.HeaderText = "Artist";
            this.Artist.Name = "Artist";
            this.Artist.ReadOnly = true;
            this.Artist.Visible = false;
            // 
            // Progress
            // 
            this.Progress.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.Progress.DataPropertyName = "Progress";
            this.Progress.HeaderText = "%";
            this.Progress.Name = "Progress";
            this.Progress.ReadOnly = true;
            this.Progress.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.Progress.Width = 30;
            // 
            // TrackDB
            // 
            this.TrackDB.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.TrackDB.DataPropertyName = "TrackDB";
            this.TrackDB.HeaderText = "Track Volume";
            this.TrackDB.Name = "TrackDB";
            this.TrackDB.ReadOnly = true;
            this.TrackDB.Width = 50;
            // 
            // Clipping
            // 
            this.Clipping.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.Clipping.DataPropertyName = "Clipping";
            this.Clipping.HeaderText = "Clip";
            this.Clipping.Name = "Clipping";
            this.Clipping.ReadOnly = true;
            this.Clipping.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Clipping.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.Clipping.Width = 49;
            // 
            // TrackFinal
            // 
            this.TrackFinal.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.TrackFinal.DataPropertyName = "TrackFinal";
            this.TrackFinal.HeaderText = "Track Gain";
            this.TrackFinal.Name = "TrackFinal";
            this.TrackFinal.ReadOnly = true;
            this.TrackFinal.Width = 50;
            // 
            // TrackClip
            // 
            this.TrackClip.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.TrackClip.DataPropertyName = "TrackClipping";
            this.TrackClip.HeaderText = "Track Clip";
            this.TrackClip.Name = "TrackClip";
            this.TrackClip.ReadOnly = true;
            this.TrackClip.Width = 50;
            // 
            // AlbumDB
            // 
            this.AlbumDB.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.AlbumDB.DataPropertyName = "AlbumDB";
            this.AlbumDB.HeaderText = "Album Volume";
            this.AlbumDB.Name = "AlbumDB";
            this.AlbumDB.ReadOnly = true;
            this.AlbumDB.Width = 50;
            // 
            // AlbumGain
            // 
            this.AlbumGain.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.AlbumGain.DataPropertyName = "AlbumFinal";
            this.AlbumGain.HeaderText = "Album Gain";
            this.AlbumGain.Name = "AlbumGain";
            this.AlbumGain.ReadOnly = true;
            this.AlbumGain.Width = 50;
            // 
            // AlbumClip
            // 
            this.AlbumClip.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.AlbumClip.DataPropertyName = "AlbumClipping";
            this.AlbumClip.HeaderText = "Album Clip";
            this.AlbumClip.Name = "AlbumClip";
            this.AlbumClip.ReadOnly = true;
            this.AlbumClip.Width = 50;
            // 
            // ErrorMessage
            // 
            this.ErrorMessage.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.ErrorMessage.DataPropertyName = "ErrorMessage";
            this.ErrorMessage.HeaderText = "Error";
            this.ErrorMessage.Name = "ErrorMessage";
            this.ErrorMessage.ReadOnly = true;
            this.ErrorMessage.Width = 54;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1085, 620);
            this.Controls.Add(this.filterGroupBox);
            this.Controls.Add(this.targetDbNumeric);
            this.Controls.Add(this.activityPanel);
            this.Controls.Add(this.readTagsButton);
            this.Controls.Add(this.searchButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.undoButton);
            this.Controls.Add(this.clearButton);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.fileGridView);
            this.Controls.Add(this.fileListLabel);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.folderPathTextBox);
            this.Controls.Add(this.runButton);
            this.Controls.Add(this.browseButton);
            this.MinimumSize = new System.Drawing.Size(1000, 300);
            this.Name = "MainForm";
            this.Padding = new System.Windows.Forms.Padding(9);
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "MP3Gain Multi-Threaded";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.activityPanel.ResumeLayout(false);
            this.activityPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.targetDbNumeric)).EndInit();
            this.filterGroupBox.ResumeLayout(false);
            this.filterGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.threshNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fileGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Button browseButton;
        private TextBox folderPathTextBox;
        private Button runButton;
        private Label label1;
        private MP3GainMT.User_Interface.DataGridViewBuffered fileGridView;
        private Label fileListLabel;
        private Button button1;
        private Button clearButton;
        private Button searchButton;
        private ProgressBar activityProgressBar;
        private Button readTagsButton;
        private Label activityLabel;
        private Label label2;
        private Panel activityPanel;
        private Label label3;
        private Button cancelButton;
        private NumericUpDown targetDbNumeric;
        private Label label4;
        private Label label5;
        private CheckBox clipOnlyTrackCheckBox;
        private GroupBox filterGroupBox;
        private Button undoButton;
        private NumericUpDown threshNumeric;
        private CheckBox threshCheckBox;
        private Label threshLabel;
        private CheckBox clipOnlyAlbumCheckBox;
        private DataGridViewCheckBoxColumn FullPath;
        private DataGridViewTextBoxColumn AlbumArtist;
        private DataGridViewCheckBoxColumn Updated;
        private DataGridViewTextBoxColumn Folder;
        private DataGridViewTextBoxColumn FileName;
        private DataGridViewTextBoxColumn Album;
        private DataGridViewTextBoxColumn Artist;
        private DataGridViewTextBoxColumn Progress;
        private DataGridViewTextBoxColumn TrackDB;
        private DataGridViewCheckBoxColumn Clipping;
        private DataGridViewTextBoxColumn TrackFinal;
        private DataGridViewCheckBoxColumn TrackClip;
        private DataGridViewTextBoxColumn AlbumDB;
        private DataGridViewTextBoxColumn AlbumGain;
        private DataGridViewCheckBoxColumn AlbumClip;
        private DataGridViewTextBoxColumn ErrorMessage;
        private CheckBox clipOnlyCheckBox;
    }
}

