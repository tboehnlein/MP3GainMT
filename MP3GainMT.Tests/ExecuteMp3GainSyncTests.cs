using Microsoft.VisualStudio.TestTools.UnitTesting;
using MP3GainMT.MP3Gain;
using MP3GainMT.Interfaces;
using MP3GainMT.Helpers; // For IFileRenamer
using MP3GainMT.Wrappers; 
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel; 
using System.Diagnostics;   
using System.IO; // For Path.Combine

namespace MP3GainMT.Tests
{
    [TestClass]
    public class ExecuteMp3GainSyncTests
    {
        private Mock<IProcess> _mockProcess;
        private Func<IProcess> _mockProcessFactory;
        private Mock<IFileRenamer> _mockFileRenamer; // Needed for base constructor
        private Dictionary<string, Mp3File> _mockFiles;
        private Mp3File _mockMp3File1;
        private string _dummyExecutablePath = "dummy_mp3gain.exe";
        private string _dummyArguments = "/s c"; 
        private string _dummyFolderPath = "C:\\TestSyncFolder";
        private string _dummyActionName = "SYNC_TEST_ACTION";
        private string _dummyFilePrefix = "SYNC_TestPrefix"; // Not used by Sync's ExtractActiveFile
        private string _dummyEndingText = "SYNC_TestDone";   // Not used by Sync's ProcessFileEnding

        private ExecuteMp3GainSync _executorSync;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockProcess = new Mock<IProcess>();
            _mockProcessFactory = () => _mockProcess.Object;
            _mockFileRenamer = new Mock<IFileRenamer>(); // Sync version doesn't use it, but base needs it.

            // For ExecuteMp3GainSync, the sortedFiles list is important for ExtractActiveFile.
            // We'll use a real Mp3File for simplicity in testing property setting.
            _mockMp3File1 = new Mp3File(Path.Combine(_dummyFolderPath, "syncfile1.mp3"));
            _mockFiles = new Dictionary<string, Mp3File>
            {
                { _mockMp3File1.FilePath, _mockMp3File1 }
            };
            
            _mockProcess.Setup(p => p.StartInfo).Returns(new ProcessStartInfo());
            _mockProcess.Setup(p => p.EnableRaisingEvents).Returns(true);


            _executorSync = new ExecuteMp3GainSync(
                _dummyExecutablePath,
                _dummyArguments,
                _mockFiles,
                _dummyFolderPath,
                _dummyActionName,
                _dummyFilePrefix, // Base class property
                _dummyEndingText, // Base class property
                _mockFileRenamer.Object, // For base constructor
                _mockProcessFactory      // For base constructor & this class's Execute
            );
            
            // Populate sortedFiles as Execute() would, for ExtractActiveFile test
            _executorSync.sortedFiles.Clear();
            foreach(var fPath in _mockFiles.Keys)
            {
                _executorSync.sortedFiles.Add(fPath);
            }
            if (_executorSync.sortedFiles.Any())
            {
                _executorSync.activeFile = _mockFiles[_executorSync.sortedFiles.First()];
            }
        }

        [TestMethod]
        public void Execute_NoRenaming_StartsProcess_ForSync()
        {
            // Act
            _executorSync.Execute();

            // Assert
            _mockProcess.Verify(p => p.Start(), Times.Once());
            _mockProcess.Verify(p => p.BeginOutputReadLine(), Times.Once());
            _mockProcess.Verify(p => p.BeginErrorReadLine(), Times.Once()); // Though Error handler is empty, it's still attached
            _mockProcess.Verify(p => p.WaitForExit(), Times.Once());

            // Verify IFileRenamer was NOT used (even though passed to base, Sync's Execute() doesn't use it)
            _mockFileRenamer.Verify(r => r.RandomlyRenameFiles(It.IsAny<Dictionary<string, string>>(), It.IsAny<List<string>>()), Times.Never());
            _mockFileRenamer.Verify(r => r.UndoRandomlyRenameFiles(It.IsAny<Dictionary<string, string>>()), Times.Never());
        }

        [TestMethod]
        public void ExtractActiveFile_Sync_ParsesMaxNoClipGain()
        {
            // Arrange
            // activeFile is already _mockMp3File1 from TestInitialize's setup of sortedFiles
            Assert.AreSame(_mockMp3File1, _executorSync.activeFile, "Initial activeFile setup is wrong.");

            // Sample line: "filepath.mp3	1	2	12345.0	5	6	7	54321.0"
            // Filepath must match one in _executorSync.sortedFiles for activeFile to be (re)confirmed.
            string filePathInLine = _mockMp3File1.FilePath; 
            double expectedTrackRaw = 12345.0;
            double expectedAlbumRaw = 54321.0;
            string sampleLine = $"{filePathInLine}\t1\t2\t{expectedTrackRaw}\t5\t6\t7\t{expectedAlbumRaw}";
            var eventArgs = new DataReceivedEventArgs(sampleLine);

            // Act
            _executorSync.ExtractActiveFile(eventArgs);

            // Assert
            Assert.AreSame(_mockMp3File1, _executorSync.activeFile, "Active file should remain the same or be correctly identified.");
            Assert.AreEqual(expectedTrackRaw, _mockMp3File1.MaxNoClipGainTrackRaw, 0.001);
            Assert.AreEqual(expectedAlbumRaw, _mockMp3File1.MaxNoClipGainAlbumRaw, 0.001);

            // Verify calculated dB values (simplified check, exact formula is in GetMinGain)
            // GetMinGain: Math.Log10(32767.0 / minValue) / Math.Log10(2.0) * 6.0206;
            double expectedTrackDb = Math.Log10(32767.0 / expectedTrackRaw) / Math.Log10(2.0) * 6.0206;
            double expectedAlbumDb = Math.Log10(32767.0 / expectedAlbumRaw) / Math.Log10(2.0) * 6.0206;
            Assert.AreEqual(expectedTrackDb, _mockMp3File1.MaxNoClipGainTrack, 0.001);
            Assert.AreEqual(expectedAlbumDb, _mockMp3File1.MaxNoClipGainAlbum, 0.001);
        }
        
        [TestMethod]
        public void ExtractActiveFile_Sync_HandlesTryParseFailureForGainValues()
        {
            // Arrange
            string filePathInLine = _mockMp3File1.FilePath; 
            // Invalid numbers for gain values
            string sampleLine = $"{filePathInLine}\t1\t2\tINVALID\t5\t6\t7\tALSO_INVALID";
            var eventArgs = new DataReceivedEventArgs(sampleLine);

            // Act
            _executorSync.ExtractActiveFile(eventArgs);

            // Assert
            // Default to 0 if TryParse fails as per ExecuteMp3GainSync.ExtractActiveFile logic
            Assert.AreEqual(0, _mockMp3File1.MaxNoClipGainTrackRaw, 0.001);
            Assert.AreEqual(0, _mockMp3File1.MaxNoClipGainAlbumRaw, 0.001);
            
            // Corresponding dB values (when raw is 0, GetMinGain would result in positive infinity due to Log10(32767/0))
            // The GetMinGain method should handle raw value of 0 gracefully if it can occur.
            // Original code: if (!Double.TryParse(values[3].Trim(), out double minValue)) { minValue = 0; }
            // Math.Log10(32767.0 / 0) is problematic. Let's check GetMinGain.
            // If minValue is 0, GetMinGain now returns a defined large positive value (e.g., 90.3).
            Assert.AreEqual(90.3, _mockMp3File1.MaxNoClipGainTrack, 0.001);
            Assert.AreEqual(90.3, _mockMp3File1.MaxNoClipGainAlbum, 0.001);
        }


        [TestMethod]
        public void Process_ErrorDataReceived_Sync_DoesNothing()
        {
            // Arrange
            var initialProgress = _mockMp3File1.Progress;
            var eventArgs = new DataReceivedEventArgs("Some error data");

            // Act
            _executorSync.Process_ErrorDataReceived(null, eventArgs);

            // Assert
            // Verify no change in file progress or other state modified by async error handler
            Assert.AreEqual(initialProgress, _mockMp3File1.Progress);
            // No calls to ReportProgress on worker (worker is null for sync, but good to be explicit if it were mocked)
        }

        [TestMethod]
        public void Process_OutputDataReceived_Sync_CallsExtractActiveFile()
        {
            // Arrange
            // We need a way to verify ExtractActiveFile was called.
            // Since ExtractActiveFile is virtual, we could mock ExecuteMp3GainSync itself for this one test,
            // but that's overly complex. Instead, we'll spy on the effects of ExtractActiveFile.
            // We know ExtractActiveFile modifies MaxNoClipGainTrackRaw on the active file.
            
            _mockMp3File1.MaxNoClipGainTrackRaw = -1.0; // Set a known different value

            string filePathInLine = _mockMp3File1.FilePath; 
            double expectedTrackRaw = 9876.5;
            string sampleLine = $"{filePathInLine}\t1\t2\t{expectedTrackRaw}\t5\t6\t7\t1234.5";
            var eventArgs = new DataReceivedEventArgs(sampleLine);

            // Act
            _executorSync.Process_OutputDataReceived(null, eventArgs);
            
            // Assert
            Assert.AreEqual(expectedTrackRaw, _mockMp3File1.MaxNoClipGainTrackRaw, 0.001, 
                "ExtractActiveFile should have been called and updated MaxNoClipGainTrackRaw.");
        }
    }
}
