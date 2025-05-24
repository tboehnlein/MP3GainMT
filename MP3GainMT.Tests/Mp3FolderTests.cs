using Microsoft.VisualStudio.TestTools.UnitTesting;
using MP3GainMT.MP3Gain;
using MP3GainMT.Helpers; // For IFileRenamer
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace MP3GainMT.Tests
{
    [TestClass]
    public class Mp3FolderTests
    {
        private string _testDirectory;
        private Mp3Folder.ExecuteMp3GainAsyncFactory _mockExecutorFactory;
        private Mock<ExecuteMp3GainAsync> _mockExecutor;
        private Mock<IFileRenamer> _mockFileRenamer;
        private Mock<BackgroundWorker> _mockBackgroundWorker;

        [TestInitialize]
        public void TestInitialize()
        {
            _testDirectory = Path.Combine(Path.GetTempPath(), "Mp3FolderTests", Guid.NewGuid().ToString());
            Directory.CreateDirectory(_testDirectory);

            _mockFileRenamer = new Mock<IFileRenamer>();

            // Setup mock for ExecuteMp3GainAsync. 
            // The constructor of ExecuteMp3GainAsync now takes IFileRenamer.
            _mockExecutor = new Mock<ExecuteMp3GainAsync>(
                It.IsAny<string>(), // executable
                It.IsAny<string>(), // arguments
                It.IsAny<Dictionary<string, Mp3File>>(), // files
                _mockFileRenamer.Object, // IFileRenamer
                It.IsAny<string>(), // folderPath
                It.IsAny<string>(), // action
                It.IsAny<BackgroundWorker>(), // worker
                It.IsAny<string>(), // successMessage
                It.IsAny<string>()  // errorMessage
            );
            
            _mockExecutorFactory = (exec, args, fls, renamer, path, act, wrkr, succMsg, errMsg) => _mockExecutor.Object;
            
            _mockBackgroundWorker = new Mock<BackgroundWorker>();
            _mockBackgroundWorker.Setup(bw => bw.ReportProgress(It.IsAny<int>(), It.IsAny<object>()));

            // Default setup for file renamer
            _mockFileRenamer.Setup(r => r.RandomlyRenameFiles(It.IsAny<Dictionary<string, string>>(), It.IsAny<List<string>>())).Returns(true);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            if (Directory.Exists(_testDirectory))
            {
                try
                {
                    Directory.Delete(_testDirectory, true);
                }
                catch (IOException) { System.Threading.Thread.Sleep(100); Directory.Delete(_testDirectory, true); }
                catch (UnauthorizedAccessException) { System.Threading.Thread.Sleep(100); Directory.Delete(_testDirectory, true); }
            }
        }

        private Mp3Folder CreateMp3Folder(string path = null)
        {
            return new Mp3Folder(path ?? _testDirectory, _mockExecutorFactory, _mockFileRenamer.Object);
        }

        [TestMethod]
        public void SearchFolder_WithMp3Files_PopulatesFilesAndRaisesEvent()
        {
            // Arrange
            string dummyMp3FilePath = Path.Combine(_testDirectory, "test.mp3");
            File.WriteAllText(dummyMp3FilePath, "dummy MP3 content");

            var folder = CreateMp3Folder();
            Mp3File foundMp3File = null;
            int eventCount = 0;
            folder.FoundFile += (sender, mp3FileArgs) => { eventCount++; foundMp3File = mp3FileArgs; };

            // Act
            folder.SearchFolder();

            // Assert
            Assert.AreEqual(1, folder.Files.Count);
            Assert.IsTrue(folder.Files.ContainsKey(dummyMp3FilePath));
            Assert.AreEqual(1, eventCount);
            Assert.AreEqual(dummyMp3FilePath, foundMp3File.FilePath);
        }

        [TestMethod]
        public void SearchFolder_NoMp3Files_FilesIsEmptyAndEventNotRaised()
        {
            File.WriteAllText(Path.Combine(_testDirectory, "test.txt"), "dummy text content");
            var folder = CreateMp3Folder();
            int eventCount = 0;
            folder.FoundFile += (sender, e) => eventCount++;
            folder.SearchFolder();
            Assert.AreEqual(0, folder.Files.Count);
            Assert.AreEqual(0, eventCount);
        }

        [TestMethod]
        public void SearchFolder_EmptyDirectory_FilesIsEmptyAndEventNotRaised()
        {
            var folder = CreateMp3Folder();
            int eventCount = 0;
            folder.FoundFile += (sender, e) => eventCount++;
            folder.SearchFolder();
            Assert.AreEqual(0, folder.Files.Count);
            Assert.AreEqual(0, eventCount);
        }

        [TestMethod]
        public void SearchFolder_NonExistentPath_FilesIsEmpty()
        {
            string nonExistentPath = Path.Combine(Path.GetTempPath(), "Mp3FolderTests", Guid.NewGuid().ToString());
            var folder = CreateMp3Folder(nonExistentPath); // Pass nonExistentPath here
            int eventCount = 0;
            folder.FoundFile += (sender, e) => eventCount++;
            try { folder.SearchFolder(); } catch (DirectoryNotFoundException) { /* Expected */ }
            Assert.AreEqual(0, folder.Files.Count);
            Assert.AreEqual(0, eventCount);
        }

        [TestMethod]
        public void ApplyGainFolder_CorrectlyCallsExecutor()
        {
            // Arrange
            var folder = CreateMp3Folder();
            var dummyFile = new Mp3File(Path.Combine(_testDirectory, "test.mp3"));
            folder.Files.Add(dummyFile.FilePath, dummyFile);
            folder.SuggestedGain = 5; // Example suggested gain

            string capturedExecutable = null;
            string capturedArguments = null;
            string capturedAction = null;

            _mockExecutorFactory = (exec, args, fls, renamer, path, act, wrkr, succMsg, errMsg) =>
            {
                capturedExecutable = exec;
                capturedArguments = args;
                capturedAction = act;
                return _mockExecutor.Object;
            };
            // Recreate folder with the capturing factory
            folder = new Mp3Folder(_testDirectory, _mockExecutorFactory, _mockFileRenamer.Object);
            folder.Files.Add(dummyFile.FilePath, dummyFile); // Re-add file
            folder.SuggestedGain = 5; // Re-set gain


            // Act
            folder.ApplyGainFolder("dummy_mp3gain_path.exe", _mockBackgroundWorker.Object);

            // Assert
            _mockExecutor.Verify(e => e.Execute(), Times.Once());
            Assert.AreEqual("dummy_mp3gain_path.exe", capturedExecutable);
            Assert.IsTrue(capturedArguments.Contains($"/g {folder.SuggestedGain}"));
            Assert.AreEqual("APPLY GAIN", capturedAction);
        }

        [TestMethod]
        public void UndoGain_CorrectlyCallsExecutor()
        {
            // Arrange
            var folder = CreateMp3Folder();
            var dummyFile = new Mp3File(Path.Combine(_testDirectory, "test.mp3"));
            folder.Files.Add(dummyFile.FilePath, dummyFile);

            string capturedExecutable = null;
            string capturedArguments = null;
            string capturedAction = null;
            
            _mockExecutorFactory = (exec, args, fls, renamer, path, act, wrkr, succMsg, errMsg) =>
            {
                capturedExecutable = exec;
                capturedArguments = args;
                capturedAction = act;
                return _mockExecutor.Object;
            };
            // Recreate folder with the capturing factory
            folder = new Mp3Folder(_testDirectory, _mockExecutorFactory, _mockFileRenamer.Object);
            folder.Files.Add(dummyFile.FilePath, dummyFile); // Re-add file

            // Act
            folder.UndoGain("dummy_mp3gain_path.exe", _mockBackgroundWorker.Object);

            // Assert
            _mockExecutor.Verify(e => e.Execute(), Times.Once());
            Assert.AreEqual("dummy_mp3gain_path.exe", capturedExecutable);
            Assert.IsTrue(capturedArguments.Contains("/u"));
            Assert.AreEqual("UNDO GAIN", capturedAction);
        }

        [TestMethod]
        public void AnalyzeGainFolder_CorrectlyCallsFileRenamerAndRuns()
        {
            // Arrange
            var folder = CreateMp3Folder();
            string dummyMp3FilePath = Path.Combine(_testDirectory, "test.mp3");
            File.WriteAllText(dummyMp3FilePath, "dummy MP3 content"); // Create a file so Files list is not empty
            folder.SearchFolder(); // Populate folder.Files

            // Act
            try
            {
                // AnalyzeGainFolder calls ExecuteAnalyzeGain, which uses Process directly.
                // We are testing if it calls IFileRenamer and doesn't crash.
                folder.AnalyzeGainFolder("dummy_mp3gain_path.exe", _mockBackgroundWorker.Object);
            }
            catch (Exception ex) when (ex.Message.Contains("Cannot start process")) // More specific if possible
            {
                // Expected if dummy_mp3gain_path.exe doesn't exist or can't be executed in test env.
                // This is okay for this test as we're primarily verifying IFileRenamer calls.
            }


            // Assert
            // Verify that RandomlyRenameFiles was called because there are files.
            _mockFileRenamer.Verify(r => r.RandomlyRenameFiles(It.IsAny<Dictionary<string, string>>(), It.Is<List<string>>(l => l.Count > 0)), Times.Once());
            // Verify that UndoRandomlyRenameFiles was called.
            _mockFileRenamer.Verify(r => r.UndoRandomlyRenameFiles(It.IsAny<Dictionary<string, string>>()), Times.Once());
            
            // Note: Deeper testing of ExecuteAnalyzeGain (e.g., Process start, output handling)
            // is complex due to its direct use of System.Diagnostics.Process and would require
            // more significant refactoring of ExecuteAnalyzeGain itself (e.g., to inject a ProcessFactory).
            // This test focuses on the IFileRenamer interaction, which was the specific refactoring target.
        }
    }
}
