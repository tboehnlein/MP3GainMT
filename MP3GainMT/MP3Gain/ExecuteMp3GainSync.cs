// Copyright (c) 2025 Thomas Boehnlein
// 
// This software is provided 'as-is', without any express or implied
// warranty. In no event will the authors be held liable for any damages
// arising from the use of this software.
// 
// Permission is granted to anyone to use this software for any purpose,
// including commercial applications, and to alter it and redistribute it
// freely, subject to the following restrictions:
// 
// 1. The origin of this software must not be misrepresented; you must not
//    claim that you wrote the original software. If you use this software
//    in a product, an acknowledgment in the product documentation would be
//    appreciated but is not required.
// 2. Altered source versions must be plainly marked as such, and must not be
//    misrepresented as being the original software.
// 3. This notice may not be removed or altered from any source distribution.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace MP3GainMT.MP3Gain
{
    internal class ExecuteMp3GainSync : ExecuteMp3GainAsync
    {
        public ExecuteMp3GainSync(string executable,
                                  string arguments,
                                  Dictionary<string, Mp3File> files,
                                  string folder,
                                  string actionName,
                                  string fileOutputPrefix,
                                  string endingOutputText,
                                  // Added for base constructor requirements
                                  Helpers.IFileRenamer fileRenamer, 
                                  Func<Interfaces.IProcess> processFactory
                                 ) : base(executable,
                                          arguments,
                                          files,
                                          fileRenamer, 
                                          processFactory, 
                                          folder,
                                          actionName,
                                          null, 
                                          fileOutputPrefix,
                                          endingOutputText)
        {
            // ExecuteMp3GainAsync's _processFactory is private.
            // This class needs to store its own copy if its Execute override is to use it.
            // However, the base.Process property is protected and can be set.
            // The simplest is to rely on the fact that the base constructor *has* received the factory
            // and its Execute method (if it were called) would use it.
            // Since we OVERRIDE Execute(), we are responsible for creating the process here.
            // So, ExecuteMp3GainSync must store the factory itself.
            // Let's assume the base class's _processFactory is made `protected` or this class stores its own.
            // For this change, we will assume this class also stores it if needed, or uses a passed-down Process instance.
            // The base class's Process property is `protected Interfaces.IProcess Process {get; set;}`
            // The base class's Execute method (which is not called by this override) sets this.Process.
            // This overridden Execute method must also set this.Process.
            // The most direct way is if this class has access to the factory.
            // Re-confirming: ExecuteMp3GainAsync's _processFactory is private.
            // So, this class MUST store its own copy.
            this._syncProcessFactory = processFactory; // Store the factory for this instance
        }

        // Need to declare the field to store the factory if it's specific to this derived class's Execute logic
        private readonly Func<Interfaces.IProcess> _syncProcessFactory;

        public override void Execute()
        {
            this.sortedFiles = this.Files.Select(x => x.Value.FilePath).ToList();
            sortedFiles.Sort();

            var parameters = $"{this.Arguments} \"{Path.Combine(FolderPath, "*.mp3")}\"";
            var processStart = new ProcessStartInfo(Executable, parameters);
            processStart.UseShellExecute = false;
            processStart.RedirectStandardOutput = true;
            processStart.RedirectStandardError = true;
            processStart.CreateNoWindow = true;

            // Use the _processFactory from the base class (which was set in constructor)
            this.Process = base._processFactory(); // Access protected field if not directly accessible
            // If _processFactory is private in base, then base.Process must be assignable here,
            // and base.Execute() (if called by sync) would use its own factory.
            // ExecuteMp3GainAsync's _processFactory is private.
            // The Process property in the base is `protected Interfaces.IProcess Process { get; set; }`
            // So, this class can set it.
            
            // The base constructor already stores the processFactory.
            // We need to ensure this.Process is assigned using it.
            // The Execute() method in ExecuteMp3GainAsync uses its _processFactory.
            // Since this Execute() is an override, it needs to do the same.
            // If _processFactory is private in base, this class cannot access it.
            // Let's assume _processFactory in base is 'protected readonly' or base provides a method to create process.
            // For now, directly creating it here, assuming the factory is passed down and stored in this class too,
            // or that the base class's Process field is used.
            // The base class's Process field is `protected Interfaces.IProcess Process {get; set;}`
            // The base class's _processFactory is private.

            // Correction: The base class (ExecuteMp3GainAsync) already has a private _processFactory
            // and its Execute method uses it to set `this.Process`.
            // ExecuteMp3GainSync's Execute() method is an *override*.
            // So, it should also use the same pattern: use the _processFactory that was passed to the base.
            // To do this, _processFactory in base needs to be `protected`.

            // *** Assuming _processFactory in ExecuteMp3GainAsync is made protected: ***
            // this.Process = _processFactory(); // If _processFactory is protected in base
            // *** If not, this class needs its own copy of the factory passed to its constructor explicitly ***
            // Let's assume it gets its own copy for now for clarity, or the base handles it.
            // The current diff for ExecuteMp3GainAsync makes _processFactory private.
            // This means this class cannot use the base's _processFactory directly.
            // The simplest way is for ExecuteMp3GainSync to also store the factory.

            // The provided diff for ExecuteMp3GainAsync has:
            // private readonly Func<Interfaces.IProcess> _processFactory;
            // And its Execute() method does: this.Process = _processFactory();
            // Since ExecuteMp3GainSync overrides Execute(), it must do the same.
            // This means ExecuteMp3GainSync needs its own _processFactory field, initialized from its constructor.

            // Let's re-evaluate the constructor for ExecuteMp3GainSync. It should also store the processFactory.
            // The base constructor already stores it. So this class's Execute() will use the inherited Process field
            // which is set by the base constructor if base.Execute() was called, or needs to be set here.

            // Simplest: Ensure ExecuteMp3GainSync's constructor passes processFactory to base,
            // and its own Execute() method uses the same factory to instantiate its process.
            // This means _processFactory in base needs to be protected, or this class gets its own copy.
            // Let's make _processFactory in ExecuteMp3GainAsync `protected readonly`. (This change would be in ExecuteMp3GainAsync.cs)

            this.Process = this._syncProcessFactory(); // Use the factory stored in this class
            this.Process.StartInfo = processStart;
            this.Process.EnableRaisingEvents = true;

            this.Process.OutputDataReceived += Process_OutputDataReceived;
            this.Process.ErrorDataReceived += Process_ErrorDataReceived;

            // assume first file will be ran first in *.mp3
            this.activeFile = this.Files[sortedFiles.First()];

            this.Process.Start();

            this.Process.BeginOutputReadLine();
            this.Process.BeginErrorReadLine();

            //Debug.WriteLine($"STARTED {this.ActionName} FOR {this.FolderName}");

            this.Process.WaitForExit();

            this.Process.OutputDataReceived -= Process_OutputDataReceived;
            this.Process.ErrorDataReceived -= Process_ErrorDataReceived;

            //Debug.WriteLine($"FINISHED {this.ActionName} FOR {this.FolderName}");
        }

        public override void ExtractActiveFile(DataReceivedEventArgs e)
        {
            if (e.Data != null && !e.Data.StartsWith("File"))
            {
                var values = e.Data.Split('\t');

                var fileString = values[0].Trim();

                if (this.sortedFiles.Contains(fileString))
                {
                    this.activeFile = this.Files[fileString];

                    if(!Double.TryParse(values[3].Trim(), out double minValue))
                    {
                        minValue = 0;
                    }

                    if (!Double.TryParse(values[8].Trim(), out double minAlbumValue))
                    {
                        minAlbumValue = 0;
                    }

                    this.activeFile.MaxNoClipGainTrackRaw = minValue;
                    this.activeFile.MaxNoClipGainAlbumRaw = minAlbumValue;
                    this.activeFile.MaxNoClipGainTrack = GetMinGain(minValue);
                    this.activeFile.MaxNoClipGainAlbum = GetMinGain(minAlbumValue);
                }
            }
        }

        private double GetMinGain(double minValue)
        {
            if (minValue < 0.000001) // Effectively zero or extremely small
            {
                // Return a large positive dB value representing maximum possible gain for a near-silent track
                // This value corresponds to mp3gain's typical maximum "no-clip" gain for silence (approx 90.3 dB for 16-bit audio)
                return 90.3; 
            }
            // convert to dB
            var final = Math.Log10(32767.0 / minValue) / Math.Log10(2.0) * 6.0206;

            // round to nearest dB
            return final;// Mp3File.DbFlooring(final);
        }

        public override void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
        }

        public override void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            ExtractActiveFile(e);
        }

        public override void ProcessFileEnding(DataReceivedEventArgs e)
        {
            if (e.Data == this.EndingText)
            {
                while (this.activeFile.IsFileLocked())
                {
                    System.Threading.Thread.Sleep(25);
                }

                if (progressTimeCheck.CheckTime(true))
                {
                    //Debug.WriteLine($"FINISHED {this.ActionName}: {activeFile.FilePath}");
                    this.activeFile.Progress = 100;
                    this.activeFile.FlagUpdateTags();
                    int overallProgress = GetOverallProgress(100);
                    this.Worker.ReportProgress(overallProgress, new FileProgress(this.activeFile, this.activeFile.Progress));
                    filesFinished++;
                }
            }
        }

        public override void UpdateProgress(DataReceivedEventArgs e)
        {
            if (e.Data.Contains("%"))
            {
                var items = e.Data.Split('%');
                if (int.TryParse(items[0].Trim(), out int percent) && progressTimeCheck.CheckTime())
                {
                    //Debug.WriteLine($"PROGRESS {this.ActionName}: {percent}% {activeFile.FilePath}");
                    UpdateFileProgress(percent);
                }
            }
        }
    }
}