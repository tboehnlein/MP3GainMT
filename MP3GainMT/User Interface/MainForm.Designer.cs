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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.browseButton = new System.Windows.Forms.Button();
            this.folderPathTextBox = new System.Windows.Forms.TextBox();
            this.applyGainButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.fileListLabel = new System.Windows.Forms.Label();
            this.analyzeButton = new System.Windows.Forms.Button();
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
            this.orRadioButton = new System.Windows.Forms.RadioButton();
            this.andRadioButton = new System.Windows.Forms.RadioButton();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.searchTextBox = new System.Windows.Forms.TextBox();
            this.clipOnlyCheckBox = new System.Windows.Forms.CheckBox();
            this.threshNumeric = new System.Windows.Forms.NumericUpDown();
            this.threshCheckBox = new System.Windows.Forms.CheckBox();
            this.clipOnlyAlbumCheckBox = new System.Windows.Forms.CheckBox();
            this.removeButton = new System.Windows.Forms.Button();
            this.threshLabel = new System.Windows.Forms.Label();
            this.undoButton = new System.Windows.Forms.Button();
            this.coresComboBox = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.mp3GainButton = new System.Windows.Forms.Button();
            this.mp3GainLabel = new System.Windows.Forms.Label();
            this.userInterfaceGroupBox = new System.Windows.Forms.GroupBox();
            this.tableFontSizeNumeric = new System.Windows.Forms.NumericUpDown();
            this.tableFontSizeUnitLabel = new System.Windows.Forms.Label();
            this.tableFontSizeLabel = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.themeComboBox = new System.Windows.Forms.ComboBox();
            this.openFolderRadioButton = new System.Windows.Forms.RadioButton();
            this.playFileRadioButton = new System.Windows.Forms.RadioButton();
            this.doubleClickRowLabel = new System.Windows.Forms.Label();
            this.readOnlyCheckBox1 = new MP3GainMT.User_Interface.ReadOnlyCheckBox();
            this.fileGridView = new MP3GainMT.User_Interface.DataGridViewBuffered();
            this.HasGainTags = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.FullPath = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AlbumColorColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.AlbumArtist = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Updated = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Folder = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FileName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Album = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Artist = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Progress = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TrackDB = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Clipping = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.TrackGain = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TrackClip = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.AlbumDB = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AlbumGain = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AlbumClip = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.NoClipGain = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ErrorMessage = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.activityPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.targetDbNumeric)).BeginInit();
            this.filterGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.threshNumeric)).BeginInit();
            this.userInterfaceGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tableFontSizeNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fileGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // browseButton
            // 
            this.browseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.browseButton.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.browseButton.FlatAppearance.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.browseButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.browseButton.Location = new System.Drawing.Point(799, 57);
            this.browseButton.Name = "browseButton";
            this.browseButton.Size = new System.Drawing.Size(70, 24);
            this.browseButton.TabIndex = 2;
            this.browseButton.Text = "&Browse...";
            this.browseButton.UseVisualStyleBackColor = false;
            this.browseButton.Click += new System.EventHandler(this.BrowseButton_Click);
            // 
            // folderPathTextBox
            // 
            this.folderPathTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.folderPathTextBox.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.folderPathTextBox.Location = new System.Drawing.Point(120, 59);
            this.folderPathTextBox.Name = "folderPathTextBox";
            this.folderPathTextBox.ReadOnly = true;
            this.folderPathTextBox.Size = new System.Drawing.Size(673, 20);
            this.folderPathTextBox.TabIndex = 1;
            // 
            // applyGainButton
            // 
            this.applyGainButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.applyGainButton.BackColor = System.Drawing.SystemColors.ControlLight;
            this.applyGainButton.FlatAppearance.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.applyGainButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.applyGainButton.Location = new System.Drawing.Point(1353, 57);
            this.applyGainButton.Name = "applyGainButton";
            this.applyGainButton.Size = new System.Drawing.Size(87, 24);
            this.applyGainButton.TabIndex = 6;
            this.applyGainButton.Text = "Apply &Gain";
            this.applyGainButton.UseVisualStyleBackColor = false;
            this.applyGainButton.Click += new System.EventHandler(this.ApplyGainButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(122, 41);
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
            // analyzeButton
            // 
            this.analyzeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.analyzeButton.BackColor = System.Drawing.SystemColors.ControlLight;
            this.analyzeButton.FlatAppearance.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.analyzeButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.analyzeButton.Location = new System.Drawing.Point(1285, 57);
            this.analyzeButton.Name = "analyzeButton";
            this.analyzeButton.Size = new System.Drawing.Size(62, 24);
            this.analyzeButton.TabIndex = 5;
            this.analyzeButton.Text = "&Analyze";
            this.analyzeButton.UseVisualStyleBackColor = false;
            this.analyzeButton.Click += new System.EventHandler(this.AnalyzeButton_Click);
            // 
            // clearButton
            // 
            this.clearButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.clearButton.BackColor = System.Drawing.SystemColors.ControlLight;
            this.clearButton.FlatAppearance.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.clearButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.clearButton.Location = new System.Drawing.Point(975, 57);
            this.clearButton.Name = "clearButton";
            this.clearButton.Size = new System.Drawing.Size(76, 24);
            this.clearButton.TabIndex = 7;
            this.clearButton.Text = "&Clear Files";
            this.clearButton.UseVisualStyleBackColor = false;
            this.clearButton.Click += new System.EventHandler(this.ClearButton_Click);
            // 
            // searchButton
            // 
            this.searchButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.searchButton.BackColor = System.Drawing.SystemColors.ControlLight;
            this.searchButton.FlatAppearance.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.searchButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.searchButton.Location = new System.Drawing.Point(882, 57);
            this.searchButton.Name = "searchButton";
            this.searchButton.Size = new System.Drawing.Size(87, 24);
            this.searchButton.TabIndex = 3;
            this.searchButton.Text = "Add &Files";
            this.searchButton.UseVisualStyleBackColor = false;
            this.searchButton.Click += new System.EventHandler(this.SearchButton_Click);
            // 
            // activityProgressBar
            // 
            this.activityProgressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.activityProgressBar.Location = new System.Drawing.Point(1294, 5);
            this.activityProgressBar.Name = "activityProgressBar";
            this.activityProgressBar.Size = new System.Drawing.Size(277, 19);
            this.activityProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.activityProgressBar.TabIndex = 11;
            // 
            // readTagsButton
            // 
            this.readTagsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.readTagsButton.BackColor = System.Drawing.SystemColors.ControlLight;
            this.readTagsButton.FlatAppearance.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.readTagsButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.readTagsButton.Location = new System.Drawing.Point(1057, 57);
            this.readTagsButton.Name = "readTagsButton";
            this.readTagsButton.Size = new System.Drawing.Size(87, 24);
            this.readTagsButton.TabIndex = 4;
            this.readTagsButton.Text = "&Read Tags";
            this.readTagsButton.UseVisualStyleBackColor = false;
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
            this.activityLabel.Size = new System.Drawing.Size(1156, 17);
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
            this.activityPanel.Size = new System.Drawing.Size(1576, 29);
            this.activityPanel.TabIndex = 12;
            this.activityPanel.Tag = "Instruction";
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(1212, 7);
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
            this.cancelButton.BackColor = System.Drawing.SystemColors.ControlLight;
            this.cancelButton.FlatAppearance.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.cancelButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cancelButton.Location = new System.Drawing.Point(1446, 57);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(55, 24);
            this.cancelButton.TabIndex = 7;
            this.cancelButton.Text = "Ca&ncel";
            this.cancelButton.UseVisualStyleBackColor = false;
            this.cancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // targetDbNumeric
            // 
            this.targetDbNumeric.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.targetDbNumeric.DecimalPlaces = 1;
            this.targetDbNumeric.Increment = new decimal(new int[] {
            15,
            0,
            0,
            65536});
            this.targetDbNumeric.Location = new System.Drawing.Point(1167, 59);
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
            this.label4.Location = new System.Drawing.Point(1172, 41);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(38, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Target";
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(1219, 63);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(20, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "dB";
            // 
            // clipOnlyTrackCheckBox
            // 
            this.clipOnlyTrackCheckBox.AutoSize = true;
            this.clipOnlyTrackCheckBox.Location = new System.Drawing.Point(63, 31);
            this.clipOnlyTrackCheckBox.Name = "clipOnlyTrackCheckBox";
            this.clipOnlyTrackCheckBox.Size = new System.Drawing.Size(74, 17);
            this.clipOnlyTrackCheckBox.TabIndex = 14;
            this.clipOnlyTrackCheckBox.Text = "Trac&k Clip";
            this.clipOnlyTrackCheckBox.UseVisualStyleBackColor = true;
            this.clipOnlyTrackCheckBox.CheckedChanged += new System.EventHandler(this.ClipOnlyCheckBox_CheckedChanged);
            // 
            // filterGroupBox
            // 
            this.filterGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.filterGroupBox.Controls.Add(this.orRadioButton);
            this.filterGroupBox.Controls.Add(this.andRadioButton);
            this.filterGroupBox.Controls.Add(this.label7);
            this.filterGroupBox.Controls.Add(this.label6);
            this.filterGroupBox.Controls.Add(this.searchTextBox);
            this.filterGroupBox.Controls.Add(this.clipOnlyCheckBox);
            this.filterGroupBox.Controls.Add(this.threshNumeric);
            this.filterGroupBox.Controls.Add(this.threshCheckBox);
            this.filterGroupBox.Controls.Add(this.clipOnlyAlbumCheckBox);
            this.filterGroupBox.Controls.Add(this.clipOnlyTrackCheckBox);
            this.filterGroupBox.Controls.Add(this.removeButton);
            this.filterGroupBox.Controls.Add(this.threshLabel);
            this.filterGroupBox.Location = new System.Drawing.Point(611, 86);
            this.filterGroupBox.MinimumSize = new System.Drawing.Size(675, 0);
            this.filterGroupBox.Name = "filterGroupBox";
            this.filterGroupBox.Size = new System.Drawing.Size(953, 57);
            this.filterGroupBox.TabIndex = 15;
            this.filterGroupBox.TabStop = false;
            this.filterGroupBox.Text = "Filter Options";
            // 
            // orRadioButton
            // 
            this.orRadioButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.orRadioButton.AutoSize = true;
            this.orRadioButton.Location = new System.Drawing.Point(901, 30);
            this.orRadioButton.Name = "orRadioButton";
            this.orRadioButton.Size = new System.Drawing.Size(43, 17);
            this.orRadioButton.TabIndex = 17;
            this.orRadioButton.TabStop = true;
            this.orRadioButton.Text = "A&ny";
            this.orRadioButton.UseVisualStyleBackColor = true;
            this.orRadioButton.CheckedChanged += new System.EventHandler(this.SearchRadio_CheckChanged);
            // 
            // andRadioButton
            // 
            this.andRadioButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.andRadioButton.AutoSize = true;
            this.andRadioButton.Checked = true;
            this.andRadioButton.Location = new System.Drawing.Point(864, 30);
            this.andRadioButton.Name = "andRadioButton";
            this.andRadioButton.Size = new System.Drawing.Size(36, 17);
            this.andRadioButton.TabIndex = 17;
            this.andRadioButton.TabStop = true;
            this.andRadioButton.Text = "A&ll";
            this.andRadioButton.UseVisualStyleBackColor = true;
            this.andRadioButton.CheckedChanged += new System.EventHandler(this.SearchRadio_CheckChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(862, 12);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(38, 13);
            this.label7.TabIndex = 16;
            this.label7.Text = "Words";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(336, 12);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(41, 13);
            this.label6.TabIndex = 16;
            this.label6.Text = "Search";
            // 
            // searchTextBox
            // 
            this.searchTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.searchTextBox.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.searchTextBox.Location = new System.Drawing.Point(332, 29);
            this.searchTextBox.Name = "searchTextBox";
            this.searchTextBox.Size = new System.Drawing.Size(452, 20);
            this.searchTextBox.TabIndex = 16;
            this.searchTextBox.TextChanged += new System.EventHandler(this.SearchTextBox_TextChanged);
            // 
            // clipOnlyCheckBox
            // 
            this.clipOnlyCheckBox.AutoSize = true;
            this.clipOnlyCheckBox.Location = new System.Drawing.Point(13, 31);
            this.clipOnlyCheckBox.Name = "clipOnlyCheckBox";
            this.clipOnlyCheckBox.Size = new System.Drawing.Size(43, 17);
            this.clipOnlyCheckBox.TabIndex = 15;
            this.clipOnlyCheckBox.Text = "Cli&p";
            this.clipOnlyCheckBox.UseVisualStyleBackColor = true;
            this.clipOnlyCheckBox.CheckedChanged += new System.EventHandler(this.ClipOnlyCheckBox_CheckedChanged);
            // 
            // threshNumeric
            // 
            this.threshNumeric.DecimalPlaces = 1;
            this.threshNumeric.Location = new System.Drawing.Point(256, 29);
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
            this.threshCheckBox.Location = new System.Drawing.Point(226, 31);
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
            this.clipOnlyAlbumCheckBox.Location = new System.Drawing.Point(144, 31);
            this.clipOnlyAlbumCheckBox.Name = "clipOnlyAlbumCheckBox";
            this.clipOnlyAlbumCheckBox.Size = new System.Drawing.Size(75, 17);
            this.clipOnlyAlbumCheckBox.TabIndex = 14;
            this.clipOnlyAlbumCheckBox.Text = "Alb&um Clip";
            this.clipOnlyAlbumCheckBox.UseVisualStyleBackColor = true;
            this.clipOnlyAlbumCheckBox.CheckedChanged += new System.EventHandler(this.ClipOnlyCheckBox_CheckedChanged);
            // 
            // removeButton
            // 
            this.removeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.removeButton.BackColor = System.Drawing.SystemColors.ControlLight;
            this.removeButton.FlatAppearance.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.removeButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.removeButton.Location = new System.Drawing.Point(790, 27);
            this.removeButton.Name = "removeButton";
            this.removeButton.Size = new System.Drawing.Size(68, 24);
            this.removeButton.TabIndex = 7;
            this.removeButton.Text = "Re&move";
            this.removeButton.UseVisualStyleBackColor = false;
            this.removeButton.Click += new System.EventHandler(this.RemoveButton_Click);
            // 
            // threshLabel
            // 
            this.threshLabel.AutoSize = true;
            this.threshLabel.Location = new System.Drawing.Point(306, 33);
            this.threshLabel.Name = "threshLabel";
            this.threshLabel.Size = new System.Drawing.Size(20, 13);
            this.threshLabel.TabIndex = 0;
            this.threshLabel.Text = "dB";
            // 
            // undoButton
            // 
            this.undoButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.undoButton.BackColor = System.Drawing.SystemColors.ControlLight;
            this.undoButton.FlatAppearance.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.undoButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.undoButton.Location = new System.Drawing.Point(1508, 57);
            this.undoButton.Name = "undoButton";
            this.undoButton.Size = new System.Drawing.Size(56, 24);
            this.undoButton.TabIndex = 7;
            this.undoButton.Text = "&Undo";
            this.undoButton.UseVisualStyleBackColor = false;
            this.undoButton.Click += new System.EventHandler(this.UndoButton_Click);
            // 
            // coresComboBox
            // 
            this.coresComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.coresComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.coresComboBox.FormattingEnabled = true;
            this.coresComboBox.Items.AddRange(new object[] {
            "3/4 Cores",
            "1/2 Cores",
            "1/4 Cores",
            "1 Cores"});
            this.coresComboBox.Location = new System.Drawing.Point(1243, 59);
            this.coresComboBox.Name = "coresComboBox";
            this.coresComboBox.Size = new System.Drawing.Size(36, 21);
            this.coresComboBox.TabIndex = 16;
            // 
            // label8
            // 
            this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(1244, 41);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(34, 13);
            this.label8.TabIndex = 0;
            this.label8.Text = "Cores";
            // 
            // mp3GainButton
            // 
            this.mp3GainButton.BackColor = System.Drawing.SystemColors.ControlLight;
            this.mp3GainButton.FlatAppearance.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.mp3GainButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mp3GainButton.Location = new System.Drawing.Point(43, 57);
            this.mp3GainButton.Name = "mp3GainButton";
            this.mp3GainButton.Size = new System.Drawing.Size(71, 24);
            this.mp3GainButton.TabIndex = 2;
            this.mp3GainButton.Text = "Bro&wse...";
            this.mp3GainButton.UseVisualStyleBackColor = false;
            this.mp3GainButton.Click += new System.EventHandler(this.MP3GainButton_Click);
            // 
            // mp3GainLabel
            // 
            this.mp3GainLabel.AutoSize = true;
            this.mp3GainLabel.CausesValidation = false;
            this.mp3GainLabel.Location = new System.Drawing.Point(16, 41);
            this.mp3GainLabel.Name = "mp3GainLabel";
            this.mp3GainLabel.Size = new System.Drawing.Size(98, 13);
            this.mp3GainLabel.TabIndex = 0;
            this.mp3GainLabel.Text = "MP3 Gain Location";
            // 
            // userInterfaceGroupBox
            // 
            this.userInterfaceGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.userInterfaceGroupBox.Controls.Add(this.tableFontSizeNumeric);
            this.userInterfaceGroupBox.Controls.Add(this.tableFontSizeUnitLabel);
            this.userInterfaceGroupBox.Controls.Add(this.tableFontSizeLabel);
            this.userInterfaceGroupBox.Controls.Add(this.label9);
            this.userInterfaceGroupBox.Controls.Add(this.themeComboBox);
            this.userInterfaceGroupBox.Controls.Add(this.openFolderRadioButton);
            this.userInterfaceGroupBox.Controls.Add(this.playFileRadioButton);
            this.userInterfaceGroupBox.Controls.Add(this.doubleClickRowLabel);
            this.userInterfaceGroupBox.Location = new System.Drawing.Point(223, 86);
            this.userInterfaceGroupBox.MinimumSize = new System.Drawing.Size(163, 0);
            this.userInterfaceGroupBox.Name = "userInterfaceGroupBox";
            this.userInterfaceGroupBox.Size = new System.Drawing.Size(383, 57);
            this.userInterfaceGroupBox.TabIndex = 18;
            this.userInterfaceGroupBox.TabStop = false;
            this.userInterfaceGroupBox.Text = "User Interface Options";
            // 
            // tableFontSizeNumeric
            // 
            this.tableFontSizeNumeric.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tableFontSizeNumeric.Location = new System.Drawing.Point(304, 30);
            this.tableFontSizeNumeric.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.tableFontSizeNumeric.Name = "tableFontSizeNumeric";
            this.tableFontSizeNumeric.Size = new System.Drawing.Size(50, 20);
            this.tableFontSizeNumeric.TabIndex = 23;
            this.tableFontSizeNumeric.Value = new decimal(new int[] {
            9,
            0,
            0,
            0});
            this.tableFontSizeNumeric.ValueChanged += new System.EventHandler(this.TableFontSizeNumeric_ValueChanged);
            // 
            // tableFontSizeUnitLabel
            // 
            this.tableFontSizeUnitLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tableFontSizeUnitLabel.AutoSize = true;
            this.tableFontSizeUnitLabel.Location = new System.Drawing.Point(356, 34);
            this.tableFontSizeUnitLabel.Name = "tableFontSizeUnitLabel";
            this.tableFontSizeUnitLabel.Size = new System.Drawing.Size(16, 13);
            this.tableFontSizeUnitLabel.TabIndex = 21;
            this.tableFontSizeUnitLabel.Text = "pt";
            // 
            // tableFontSizeLabel
            // 
            this.tableFontSizeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tableFontSizeLabel.AutoSize = true;
            this.tableFontSizeLabel.Location = new System.Drawing.Point(309, 12);
            this.tableFontSizeLabel.Name = "tableFontSizeLabel";
            this.tableFontSizeLabel.Size = new System.Drawing.Size(28, 13);
            this.tableFontSizeLabel.TabIndex = 22;
            this.tableFontSizeLabel.Text = "Font";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(230, 12);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(40, 13);
            this.label9.TabIndex = 20;
            this.label9.Text = "Theme";
            // 
            // themeComboBox
            // 
            this.themeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.themeComboBox.FormattingEnabled = true;
            this.themeComboBox.Items.AddRange(new object[] {
            "Light",
            "Dark"});
            this.themeComboBox.Location = new System.Drawing.Point(231, 29);
            this.themeComboBox.Name = "themeComboBox";
            this.themeComboBox.Size = new System.Drawing.Size(64, 21);
            this.themeComboBox.TabIndex = 19;
            this.themeComboBox.SelectedIndexChanged += new System.EventHandler(this.ThemeComboBox_SelectedIndexChanged);
            // 
            // openFolderRadioButton
            // 
            this.openFolderRadioButton.AutoSize = true;
            this.openFolderRadioButton.Checked = true;
            this.openFolderRadioButton.Location = new System.Drawing.Point(139, 30);
            this.openFolderRadioButton.Name = "openFolderRadioButton";
            this.openFolderRadioButton.Size = new System.Drawing.Size(83, 17);
            this.openFolderRadioButton.TabIndex = 17;
            this.openFolderRadioButton.TabStop = true;
            this.openFolderRadioButton.Text = "&Open Folder";
            this.openFolderRadioButton.UseVisualStyleBackColor = true;
            this.openFolderRadioButton.CheckedChanged += new System.EventHandler(this.DoubleClickChoice_Changed);
            // 
            // playFileRadioButton
            // 
            this.playFileRadioButton.AutoSize = true;
            this.playFileRadioButton.Location = new System.Drawing.Point(75, 30);
            this.playFileRadioButton.Name = "playFileRadioButton";
            this.playFileRadioButton.Size = new System.Drawing.Size(64, 17);
            this.playFileRadioButton.TabIndex = 17;
            this.playFileRadioButton.Text = "&Play File";
            this.playFileRadioButton.UseVisualStyleBackColor = true;
            this.playFileRadioButton.CheckedChanged += new System.EventHandler(this.DoubleClickChoice_Changed);
            // 
            // doubleClickRowLabel
            // 
            this.doubleClickRowLabel.AutoSize = true;
            this.doubleClickRowLabel.Location = new System.Drawing.Point(6, 31);
            this.doubleClickRowLabel.Name = "doubleClickRowLabel";
            this.doubleClickRowLabel.Size = new System.Drawing.Size(67, 13);
            this.doubleClickRowLabel.TabIndex = 16;
            this.doubleClickRowLabel.Text = "Double Click";
            // 
            // readOnlyCheckBox1
            // 
            this.readOnlyCheckBox1.AutoSize = true;
            this.readOnlyCheckBox1.ForeColor = System.Drawing.Color.Gray;
            this.readOnlyCheckBox1.Location = new System.Drawing.Point(25, 63);
            this.readOnlyCheckBox1.Name = "readOnlyCheckBox1";
            this.readOnlyCheckBox1.ReadOnly = true;
            this.readOnlyCheckBox1.Size = new System.Drawing.Size(15, 14);
            this.readOnlyCheckBox1.TabIndex = 17;
            this.readOnlyCheckBox1.UseVisualStyleBackColor = true;
            // 
            // fileGridView
            // 
            this.fileGridView.AllowDrop = true;
            this.fileGridView.AllowUserToAddRows = false;
            this.fileGridView.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            this.fileGridView.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.fileGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fileGridView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.fileGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.ControlLight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.fileGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.fileGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.fileGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.HasGainTags,
            this.FullPath,
            this.AlbumColorColumn,
            this.AlbumArtist,
            this.Updated,
            this.Folder,
            this.FileName,
            this.Album,
            this.Artist,
            this.Progress,
            this.TrackDB,
            this.Clipping,
            this.TrackGain,
            this.TrackClip,
            this.AlbumDB,
            this.AlbumGain,
            this.AlbumClip,
            this.NoClipGain,
            this.ErrorMessage});
            this.fileGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.fileGridView.EnableHeadersVisualStyles = false;
            this.fileGridView.GridColor = System.Drawing.Color.Black;
            this.fileGridView.Location = new System.Drawing.Point(12, 162);
            this.fileGridView.MultiSelect = false;
            this.fileGridView.Name = "fileGridView";
            this.fileGridView.ReadOnly = true;
            this.fileGridView.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.fileGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.fileGridView.Size = new System.Drawing.Size(1552, 690);
            this.fileGridView.TabIndex = 9;
            this.fileGridView.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.FileGridView_CellFormatting);
            this.fileGridView.CellToolTipTextNeeded += new System.Windows.Forms.DataGridViewCellToolTipTextNeededEventHandler(this.FileGridView_CellToolTipTextNeeded);
            this.fileGridView.RowPrePaint += new System.Windows.Forms.DataGridViewRowPrePaintEventHandler(this.FileGridView_RowPrePaint);
            this.fileGridView.Scroll += new System.Windows.Forms.ScrollEventHandler(this.FileGridView_Scroll);
            this.fileGridView.DragDrop += new System.Windows.Forms.DragEventHandler(this.FileGridView_DragDrop);
            this.fileGridView.DragEnter += new System.Windows.Forms.DragEventHandler(this.FileGridView_DragEnter);
            // 
            // HasGainTags
            // 
            this.HasGainTags.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.HasGainTags.DataPropertyName = "HasGainTags";
            this.HasGainTags.HeaderText = "Tags";
            this.HasGainTags.Name = "HasGainTags";
            this.HasGainTags.ReadOnly = true;
            this.HasGainTags.Visible = false;
            this.HasGainTags.Width = 35;
            // 
            // FullPath
            // 
            this.FullPath.DataPropertyName = "FullPath";
            this.FullPath.HeaderText = "Full Path";
            this.FullPath.MinimumWidth = 200;
            this.FullPath.Name = "FullPath";
            this.FullPath.ReadOnly = true;
            this.FullPath.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.FullPath.Width = 585;
            // 
            // AlbumColorColumn
            // 
            this.AlbumColorColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.AlbumColorColumn.DataPropertyName = "AlbumColorAlternative";
            this.AlbumColorColumn.HeaderText = "Color";
            this.AlbumColorColumn.Name = "AlbumColorColumn";
            this.AlbumColorColumn.ReadOnly = true;
            this.AlbumColorColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.AlbumColorColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.AlbumColorColumn.Visible = false;
            this.AlbumColorColumn.Width = 54;
            // 
            // AlbumArtist
            // 
            this.AlbumArtist.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.AlbumArtist.DataPropertyName = "AlbumArtist";
            this.AlbumArtist.HeaderText = "Album Artist";
            this.AlbumArtist.Name = "AlbumArtist";
            this.AlbumArtist.ReadOnly = true;
            this.AlbumArtist.Visible = false;
            this.AlbumArtist.Width = 85;
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
            this.Folder.Visible = false;
            // 
            // FileName
            // 
            this.FileName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.FileName.DataPropertyName = "FileName";
            this.FileName.HeaderText = "File Name";
            this.FileName.Name = "FileName";
            this.FileName.ReadOnly = true;
            this.FileName.Visible = false;
            // 
            // Album
            // 
            this.Album.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.Album.DataPropertyName = "Album";
            this.Album.HeaderText = "Album";
            this.Album.Name = "Album";
            this.Album.ReadOnly = true;
            this.Album.Visible = false;
            this.Album.Width = 59;
            // 
            // Artist
            // 
            this.Artist.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.Artist.DataPropertyName = "Artist";
            this.Artist.HeaderText = "Artist";
            this.Artist.Name = "Artist";
            this.Artist.ReadOnly = true;
            this.Artist.Visible = false;
            this.Artist.Width = 53;
            // 
            // Progress
            // 
            this.Progress.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.Progress.DataPropertyName = "Progress";
            this.Progress.HeaderText = "%";
            this.Progress.Name = "Progress";
            this.Progress.ReadOnly = true;
            this.Progress.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.Progress.Width = 38;
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
            this.Clipping.Width = 47;
            // 
            // TrackGain
            // 
            this.TrackGain.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.TrackGain.DataPropertyName = "TrackGain";
            this.TrackGain.HeaderText = "Track Gain";
            this.TrackGain.Name = "TrackGain";
            this.TrackGain.ReadOnly = true;
            this.TrackGain.Width = 77;
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
            this.AlbumGain.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.AlbumGain.DataPropertyName = "AlbumGain";
            this.AlbumGain.HeaderText = "Album Gain";
            this.AlbumGain.Name = "AlbumGain";
            this.AlbumGain.ReadOnly = true;
            this.AlbumGain.Width = 77;
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
            // NoClipGain
            // 
            this.NoClipGain.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.NoClipGain.DataPropertyName = "MaxNoClip";
            this.NoClipGain.HeaderText = "No Clip";
            this.NoClipGain.Name = "NoClipGain";
            this.NoClipGain.ReadOnly = true;
            this.NoClipGain.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.NoClipGain.Width = 40;
            // 
            // ErrorMessage
            // 
            this.ErrorMessage.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ErrorMessage.DataPropertyName = "ErrorMessage";
            this.ErrorMessage.HeaderText = "Error";
            this.ErrorMessage.Name = "ErrorMessage";
            this.ErrorMessage.ReadOnly = true;
            this.ErrorMessage.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1576, 864);
            this.Controls.Add(this.userInterfaceGroupBox);
            this.Controls.Add(this.readOnlyCheckBox1);
            this.Controls.Add(this.coresComboBox);
            this.Controls.Add(this.filterGroupBox);
            this.Controls.Add(this.targetDbNumeric);
            this.Controls.Add(this.activityPanel);
            this.Controls.Add(this.readTagsButton);
            this.Controls.Add(this.searchButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.undoButton);
            this.Controls.Add(this.clearButton);
            this.Controls.Add(this.analyzeButton);
            this.Controls.Add(this.fileGridView);
            this.Controls.Add(this.fileListLabel);
            this.Controls.Add(this.mp3GainButton);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.mp3GainLabel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.folderPathTextBox);
            this.Controls.Add(this.applyGainButton);
            this.Controls.Add(this.browseButton);
            this.MinimumSize = new System.Drawing.Size(1300, 300);
            this.Name = "MainForm";
            this.Padding = new System.Windows.Forms.Padding(9);
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "MP3Gain Multi-Threaded";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            this.activityPanel.ResumeLayout(false);
            this.activityPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.targetDbNumeric)).EndInit();
            this.filterGroupBox.ResumeLayout(false);
            this.filterGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.threshNumeric)).EndInit();
            this.userInterfaceGroupBox.ResumeLayout(false);
            this.userInterfaceGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tableFontSizeNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fileGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Button browseButton;
        private TextBox folderPathTextBox;
        private Button applyGainButton;
        private Label label1;
        private MP3GainMT.User_Interface.DataGridViewBuffered fileGridView;
        private Label fileListLabel;
        private Button analyzeButton;
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
        private CheckBox clipOnlyCheckBox;
        private TextBox searchTextBox;
        private Label label6;
        private Button removeButton;
        private RadioButton orRadioButton;
        private RadioButton andRadioButton;
        private Label label7;
        private ComboBox coresComboBox;
        private Label label8;
        private Button mp3GainButton;
        private Label mp3GainLabel;
        private ReadOnlyCheckBox readOnlyCheckBox1;
        private GroupBox userInterfaceGroupBox;
        private RadioButton openFolderRadioButton;
        private RadioButton playFileRadioButton;
        private Label doubleClickRowLabel;
        private Label label9;
        private ComboBox themeComboBox;
        private NumericUpDown tableFontSizeNumeric;
        private Label tableFontSizeUnitLabel;
        private Label tableFontSizeLabel;
        private DataGridViewCheckBoxColumn HasGainTags;
        private DataGridViewTextBoxColumn FullPath;
        private DataGridViewCheckBoxColumn AlbumColorColumn;
        private DataGridViewTextBoxColumn AlbumArtist;
        private DataGridViewCheckBoxColumn Updated;
        private DataGridViewTextBoxColumn Folder;
        private DataGridViewTextBoxColumn FileName;
        private DataGridViewTextBoxColumn Album;
        private DataGridViewTextBoxColumn Artist;
        private DataGridViewTextBoxColumn Progress;
        private DataGridViewTextBoxColumn TrackDB;
        private DataGridViewCheckBoxColumn Clipping;
        private DataGridViewTextBoxColumn TrackGain;
        private DataGridViewCheckBoxColumn TrackClip;
        private DataGridViewTextBoxColumn AlbumDB;
        private DataGridViewTextBoxColumn AlbumGain;
        private DataGridViewCheckBoxColumn AlbumClip;
        private DataGridViewTextBoxColumn NoClipGain;
        private DataGridViewTextBoxColumn ErrorMessage;
    }
}

