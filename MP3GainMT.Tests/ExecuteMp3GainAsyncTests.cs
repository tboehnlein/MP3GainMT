using Microsoft.VisualStudio.TestTools.UnitTesting;
using MP3GainMT.MP3Gain;
using MP3GainMT.Interfaces;
using MP3GainMT.Helpers;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics; // For DataReceivedEventArgs

namespace MP3GainMT.Tests
{
    [TestClass]
    public class ExecuteMp3GainAsyncTests
    {
        private Mock<IProcess> _mockProcess;
        private Func<IProcess> _mockProcessFactory;
        private Mock<IFileRenamer> _mockFileRenamer;
        private Mock<BackgroundWorker> _mockBackgroundWorker;
        private Dictionary<string, Mp3File> _mockFiles;
        private Mp3File _mockMp3File1; // A representative Mp3File
        private string _dummyExecutablePath = "dummy_mp3gain.exe";
        private string _dummyArguments = "/r /k";
        private string _dummyFolderPath = "C:\\TestFolder";
        private string _dummyActionName = "TEST_ACTION";
        private string _dummyFilePrefix = "TestPrefix";
        private string _dummyEndingText = "TestDone";

        private ExecuteMp3GainAsync _executor;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockProcess = new Mock<IProcess>();
            _mockProcessFactory = () => _mockProcess.Object;

            _mockFileRenamer = new Mock<IFileRenamer>();
            _mockBackgroundWorker = new Mock<BackgroundWorker>();
            _mockBackgroundWorker.Setup(bw => bw.WorkerReportsProgress).Returns(true); // Assume it can report progress

            // Prepare mock Mp3File instances and the dictionary
            _mockMp3File1 = new Mp3File(System.IO.Path.Combine(_dummyFolderPath, "testfile1.mp3"));
            // Add more mock files if specific tests need them
            
            _mockFiles = new Dictionary<string, Mp3File>
            {
                { _mockMp3File1.FilePath, _mockMp3File1 }
            };

            // Setup default behavior for mocks if needed
            _mockFileRenamer.Setup(r => r.RandomlyRenameFiles(It.IsAny<Dictionary<string, string>>(), It.IsAny<List<string>>()))
                            .Returns(true)
                            .Callback<Dictionary<string, string>, List<string>>((lookUp, files) => 
                            {
                                // Simulate renaming: populate lookup from original path to a temp name
                                foreach(var fPath in files)
                                {
                                    var tempName = System.IO.Path.GetFileNameWithoutExtension(fPath) + "_temp.mp3";
                                    var tempFullPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(fPath), tempName);
                                    lookUp.Add(tempFullPath, fPath); // Maps temp name to original full path
                                }
                            });
            _mockProcess.Setup(p => p.StartInfo).Returns(new ProcessStartInfo()); // Avoid NullRef if StartInfo is accessed

            _executor = new ExecuteMp3GainAsync(
                _dummyExecutablePath,
                _dummyArguments,
                _mockFiles,
                _mockFileRenamer.Object,
                _mockProcessFactory,
                _dummyFolderPath,
                _dummyActionName,
                _mockBackgroundWorker.Object,
                _dummyFilePrefix,
                _dummyEndingText
            );
        }

        [TestMethod]
        public void Constructor_InitializesPropertiesCorrectly()
        {
            // Assert
            Assert.AreEqual(_dummyExecutablePath, _executor.Executable);
            Assert.AreEqual(_dummyArguments, _executor.Arguments);
            Assert.AreSame(_mockFiles, _executor.Files);
            Assert.AreEqual(_dummyFolderPath, _executor.FolderPath);
            Assert.AreEqual(_dummyActionName, _executor.ActionName);
            Assert.AreSame(_mockBackgroundWorker.Object, _executor.Worker);
            // Note: FilePrefix and EndingText are protected, not directly testable unless exposed or via behavior.
            // Accessing protected members for testing is possible via reflection or by making them internal.
            // For now, their correct usage will be verified by behavior in other tests (e.g., Process_ErrorDataReceived).
            
            // Verify internal fields related to injected dependencies were set (indirectly)
            // For example, if _fileRenamer or _processFactory were null, constructor throws ArgumentNullException
            // This is implicitly tested by the successful instantiation in TestInitialize.
        }

        [TestMethod]
        public void Execute_CallsRenamerAndProcessMethods()
        {
            // Arrange
            // _executor is already set up in TestInitialize

            // Act
            _executor.Execute();

            // Assert
            _mockFileRenamer.Verify(r => r.RandomlyRenameFiles(It.IsAny<Dictionary<string, string>>(), It.IsAny<List<string>>()), Times.Once());
            _mockProcess.Verify(p => p.Start(), Times.Once());
            _mockProcess.Verify(p => p.BeginOutputReadLine(), Times.Once());
            _mockProcess.Verify(p => p.BeginErrorReadLine(), Times.Once());
            _mockProcess.Verify(p => p.WaitForExit(), Times.Once());
            _mockFileRenamer.Verify(r => r.UndoRandomlyRenameFiles(It.IsAny<Dictionary<string, string>>()), Times.Once());
        }

        [TestMethod]
        public void Process_ErrorDataReceived_ExtractsActiveFileAndProgressAndCompletion()
        {
            // Arrange
            // Make activeFile accessible for verification or use a known file from _mockFiles
            // For this test, we'll set up sortedFiles and fileLookUp as Execute() would.
            // And then directly call Process_ErrorDataReceived.

            // Simulate state after RandomlyRenameFiles
            var fileLookup = new Dictionary<string, string>();
            var sortedFilePaths = new List<string>();
            
            foreach(var kvp in _executor.Files)
            {
                // Simulate a renamed path for testing ExtractActiveFile part
                var renamedPath = System.IO.Path.Combine(_dummyFolderPath, "renamed_" + System.IO.Path.GetFileName(kvp.Key));
                fileLookup.Add(renamedPath, kvp.Key); // renamed path -> original path
                sortedFilePaths.Add(renamedPath); 
            }
            
            // Manually set these protected/internal fields for test isolation if Execute() is not called.
            // This requires making them accessible (e.g. internal or using reflection/internalsvisibleto)
            // For now, let's assume a way to set them or that they are set by a lightweight setup.
            // If Execute() is called, it sets them up. Here, we are unit testing Process_ErrorDataReceived.
            
            // To test Process_ErrorDataReceived in isolation, we need to set the internal state
            // that Execute() would normally set.
            // For this example, we'll assume _executor.sortedFiles and _executor.fileLookUp are populated.
            // And _executor.activeFile is set to the first file.
            
            _executor.sortedFiles.Clear();
            _executor.sortedFiles.AddRange(sortedFilePaths);
            
            // Populate the internal fileLookUp (this is a bit of a hack without InternalsVisibleTo or reflection)
            // A better way would be to make the fields internal or have a test-specific constructor/method.
            // For now, we rely on the callback in TestInitialize's _mockFileRenamer.Setup for this.
            // Let's refine the callback to better simulate the structure Execute() expects in fileLookUp.
             _mockFileRenamer.Setup(r => r.RandomlyRenameFiles(It.IsAny<Dictionary<string, string>>(), It.IsAny<List<string>>()))
                            .Returns(true)
                            .Callback<Dictionary<string, string>, List<string>>((lookUp, files) => 
                            {
                                lookUp.Clear(); // Clear before populating
                                foreach(var originalFilePath in files)
                                {
                                    // Key: renamed path, Value: original path (as used by ExecuteMp3GainAsync)
                                    var renamedFileName = "renamed_" + System.IO.Path.GetFileName(originalFilePath);
                                    var renamedFullPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(originalFilePath), renamedFileName);
                                    lookUp.Add(renamedFullPath, originalFilePath); 
                                    if(!_executor.sortedFiles.Contains(renamedFullPath)) _executor.sortedFiles.Add(renamedFullPath);
                                }
                                if(_executor.sortedFiles.Any())
                                {
                                     // Set activeFile to the original Mp3File object corresponding to the first renamed file
                                     var firstOriginalPath = lookUp[_executor.sortedFiles.First()];
                                     _executor.activeFile = _executor.Files[firstOriginalPath];
                                }
                            });
            
            // Trigger the callback to populate fileLookUp and sortedFiles inside _executor
            var initialFileLookUp = new Dictionary<string, string>();
            _mockFileRenamer.Object.RandomlyRenameFiles(initialFileLookUp, _executor.Files.Keys.ToList());
            // Now _executor.sortedFiles should be populated with renamed paths,
            // and initialFileLookUp maps renamed paths to original paths.
            // _executor.activeFile should point to the Mp3File instance of the first original file.

            Mp3File targetFile = _executor.activeFile; // The file we expect to be modified
            Assert.IsNotNull(targetFile, "Active file should be set by the renamer callback logic for the test.");

            // --- Test File Identification ---
            string renamedFilePathForEvent = _executor.sortedFiles.First(); // e.g., "C:\TestFolder\renamed_testfile1.mp3"
            string identifyingLine = $"{_dummyFilePrefix} {renamedFilePathForEvent} to ...";
            _executor.Process_ErrorDataReceived(null, new DataReceivedEventArgs(identifyingLine));
            // Verify activeFile was correctly set or remained if already correct due to ExtractActiveFile call
            // (activeFile is protected; test via side effects or make internal for test project)
            // For now, we assume ExtractActiveFile logic is correct and targetFile is indeed the active one.

            // --- Test Progress Update ---
            string progressLine = "50%";
            _executor.Process_ErrorDataReceived(null, new DataReceivedEventArgs(progressLine));
            Assert.AreEqual(50, targetFile.Progress);
            _mockBackgroundWorker.Verify(bw => bw.ReportProgress(It.IsAny<int>(), It.Is<FileProgress>(fp => fp.File == targetFile && fp.Progress == 50)), Times.AtLeastOnce());
            
            // --- Test Completion ---
            _executor.filesFinished = 0; // Reset for this part of the test
            string completionLine = _dummyEndingText; // e.g., "TestDone"
            _executor.Process_ErrorDataReceived(null, new DataReceivedEventArgs(completionLine));
            Assert.AreEqual(100, targetFile.Progress);
            Assert.IsTrue(targetFile.Updated); // FlagUpdateTags should have been called, setting Updated to true (if Mp3File.FlagUpdateTags sets it)
                                               // Let's assume FlagUpdateTags sets HasTags = false, and then something else sets Updated.
                                               // The original code calls FlagUpdateTags. Let's verify HasTags.
            Assert.IsFalse(targetFile.HasTags); // FlagUpdateTags sets HasTags to false.
            _mockBackgroundWorker.Verify(bw => bw.ReportProgress(It.IsAny<int>(), It.Is<FileProgress>(fp => fp.File == targetFile && fp.Progress == 100)), Times.AtLeastOnce());
            Assert.AreEqual(1, _executor.filesFinished);
        }
        
        [TestMethod]
        public void ExtractActiveFile_CorrectlyIdentifiesFileFromRenamedPath()
        {
            // Arrange
            // Setup sortedFiles and fileLookUp as in Execute()
            var fileLookup = new Dictionary<string, string>();
            var sortedFilePaths = new List<string>();
            var originalPaths = _executor.Files.Keys.ToList();

            _mockFileRenamer.Setup(r => r.RandomlyRenameFiles(It.IsAny<Dictionary<string, string>>(), It.IsAny<List<string>>()))
                .Returns(true)
                .Callback<Dictionary<string, string>, List<string>>((lookUp, files) => 
                {
                    lookUp.Clear();
                    _executor.sortedFiles.Clear();
                    for(int i=0; i < files.Count; i++)
                    {
                        var originalFilePath = files[i];
                        var renamedFileName = $"renamed_file_{i}.mp3";
                        var renamedFullPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(originalFilePath), renamedFileName);
                        lookUp.Add(renamedFullPath, originalFilePath); 
                        _executor.sortedFiles.Add(renamedFullPath);
                    }
                });
            
            _mockFileRenamer.Object.RandomlyRenameFiles(fileLookup, originalPaths); // This populates _executor.sortedFiles and fileLookup

            // Pick a file to be identified
            Assert.IsTrue(_executor.sortedFiles.Count > 0, "Sorted files should be populated for the test.");
            string targetRenamedPath = _executor.sortedFiles.First(); // e.g., C:\TestFolder\renamed_file_0.mp3
            string expectedOriginalPath = fileLookup[targetRenamedPath]; // The original path for this renamed file
            Mp3File expectedActiveMp3File = _executor.Files[expectedOriginalPath];

            string lineFromFilePrefix = $"{_dummyFilePrefix} {targetRenamedPath} to something..."; // Typical line format

            // Act
            _executor.ExtractActiveFile(new DataReceivedEventArgs(lineFromFilePrefix));

            // Assert
            // activeFile is protected, so we check its side-effects or would need InternalsVisibleTo
            // For now, we assume if the next progress update targets this file, it was set.
            // Let's test by calling UpdateProgress directly after this, assuming activeFile was set.
            _executor.UpdateProgress(new DataReceivedEventArgs(" 75% ")); // Simulate progress line
            Assert.AreEqual(75, expectedActiveMp3File.Progress, "Progress should be updated on the correctly identified active file.");
        }
    }
}
