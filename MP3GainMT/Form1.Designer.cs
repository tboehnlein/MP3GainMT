﻿using System.Windows.Forms;

namespace MP3GainMT
{
    partial class Form1
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
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.label2 = new System.Windows.Forms.Label();
            this.FullPath = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Folder = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FileName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Progress = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TrackDB = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TrackFinal = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AlbumDB = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AlbumGain = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // browseButton
            // 
            this.browseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.browseButton.Location = new System.Drawing.Point(808, 23);
            this.browseButton.Name = "browseButton";
            this.browseButton.Size = new System.Drawing.Size(35, 28);
            this.browseButton.TabIndex = 0;
            this.browseButton.Text = "...";
            this.browseButton.UseVisualStyleBackColor = true;
            this.browseButton.Click += new System.EventHandler(this.BrowseButton_Click);
            // 
            // folderPathTextBox
            // 
            this.folderPathTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.folderPathTextBox.Location = new System.Drawing.Point(19, 27);
            this.folderPathTextBox.Name = "folderPathTextBox";
            this.folderPathTextBox.Size = new System.Drawing.Size(783, 20);
            this.folderPathTextBox.TabIndex = 1;
            // 
            // runButton
            // 
            this.runButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.runButton.Location = new System.Drawing.Point(849, 23);
            this.runButton.Name = "runButton";
            this.runButton.Size = new System.Drawing.Size(87, 28);
            this.runButton.TabIndex = 0;
            this.runButton.Text = "Apply Gain";
            this.runButton.UseVisualStyleBackColor = true;
            this.runButton.Click += new System.EventHandler(this.RunButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(19, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(151, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Parent Folder of Album Folders";
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.FullPath,
            this.Folder,
            this.FileName,
            this.Progress,
            this.TrackDB,
            this.TrackFinal,
            this.AlbumDB,
            this.AlbumGain});
            this.dataGridView1.Location = new System.Drawing.Point(12, 68);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.Size = new System.Drawing.Size(924, 310);
            this.dataGridView1.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 52);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Gain Results";
            // 
            // FullPath
            // 
            this.FullPath.DataPropertyName = "FullPath";
            this.FullPath.HeaderText = "Full Path";
            this.FullPath.Name = "FullPath";
            this.FullPath.ReadOnly = true;
            this.FullPath.Visible = false;
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
            this.FileName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
            this.FileName.DataPropertyName = "FileName";
            this.FileName.HeaderText = "File Name";
            this.FileName.Name = "FileName";
            this.FileName.ReadOnly = true;
            this.FileName.Width = 5;
            // 
            // Progress
            // 
            this.Progress.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Progress.DataPropertyName = "Progress";
            this.Progress.HeaderText = "%";
            this.Progress.Name = "Progress";
            this.Progress.ReadOnly = true;
            this.Progress.Width = 40;
            // 
            // TrackDB
            // 
            this.TrackDB.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.TrackDB.DataPropertyName = "TrackDB";
            this.TrackDB.HeaderText = "Track Before";
            this.TrackDB.Name = "TrackDB";
            this.TrackDB.ReadOnly = true;
            this.TrackDB.Width = 87;
            // 
            // TrackFinal
            // 
            this.TrackFinal.DataPropertyName = "TrackFinal";
            this.TrackFinal.HeaderText = "Track After";
            this.TrackFinal.Name = "TrackFinal";
            this.TrackFinal.ReadOnly = true;
            // 
            // AlbumDB
            // 
            this.AlbumDB.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.AlbumDB.DataPropertyName = "AlbumDB";
            this.AlbumDB.HeaderText = "Album Before";
            this.AlbumDB.Name = "AlbumDB";
            this.AlbumDB.ReadOnly = true;
            this.AlbumDB.Width = 88;
            // 
            // AlbumGain
            // 
            this.AlbumGain.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.AlbumGain.DataPropertyName = "AlbumFinal";
            this.AlbumGain.HeaderText = "Album After";
            this.AlbumGain.Name = "AlbumGain";
            this.AlbumGain.ReadOnly = true;
            this.AlbumGain.Width = 79;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(948, 390);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.folderPathTextBox);
            this.Controls.Add(this.runButton);
            this.Controls.Add(this.browseButton);
            this.Name = "Form1";
            this.Padding = new System.Windows.Forms.Padding(9);
            this.Text = "MP3Gain Multi-Threaded";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Button browseButton;
        private TextBox folderPathTextBox;
        private Button runButton;
        private Label label1;
        private DataGridView dataGridView1;
        private Label label2;
        private DataGridViewTextBoxColumn FullPath;
        private DataGridViewTextBoxColumn Folder;
        private DataGridViewTextBoxColumn FileName;
        private DataGridViewTextBoxColumn Progress;
        private DataGridViewTextBoxColumn TrackDB;
        private DataGridViewTextBoxColumn TrackFinal;
        private DataGridViewTextBoxColumn AlbumDB;
        private DataGridViewTextBoxColumn AlbumGain;
    }
}

