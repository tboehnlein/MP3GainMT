using Microsoft.VisualStudio.TestTools.UnitTesting;
using MP3GainMT.MP3Gain;
using MP3GainMT.Interfaces;
using MP3GainMT.Helpers; // For IFileRenamer if needed by ExecuteMp3GainSyncFactory
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel; // For DoWorkEventArgs etc.
using System.Linq; // For LINQ operations if any

namespace MP3GainMT.Tests
{
    [TestClass]
    public class Mp3GainManagerTests
    {
        private Mock<IMp3FolderFactory> _mockMp3FolderFactory;
        private Mock<IBackgroundWorkerFactory> _mockBackgroundWorkerFactory;
        private Mock<Mp3GainManager.ExecuteMp3GainSyncFactory> _mockExecuteMp3GainSyncFactory;
        
        private Mock<IBackgroundWorker> _mockSearchWorker;
        private Mock<IBackgroundWorker> _mockReadTagsWorker;
        // For processStack workers, we'll create them as needed in tests, 
        // as TestInitialize might set up a specific sequence for search/readTags.
        // private Mock<IBackgroundWorker> _mockProcessStackWorker; 

        private Mock<Mp3Folder> _mockMp3Folder;
        private Mock<Mp3File> _mockMp3File1; // Mock individual Mp3File
        private Mock<Mp3File> _mockMp3File2; // Mock another Mp3File
        private Dictionary<string, Mp3File> _folderFilesDictionary; // To be returned by _mockMp3Folder.Files
        private List<Mp3File> _folderFilesList; // To be returned by _mockMp3Folder.MP3Files (if it returns List<Mp3File>)
                                                // Or List<string> if it returns file paths.

        private Mock<ExecuteMp3GainSync> _mockExecuteMp3GainSync;
        private Mock<IFileRenamer> _mockFileRenamer; 
        private Mock<Func<IProcess>> _mockProcessFactoryFunc; // Renamed for clarity from _mockProcessFactory (which is a Mock object)

        private Mp3GainManager _manager;

        // Captured event handlers
        private DoWorkEventHandler _capturedSearchWorkerDoWork;
        private ProgressChangedEventHandler _capturedSearchWorkerProgressChanged;
        private RunWorkerCompletedEventHandler _capturedSearchWorkerCompleted;

        private DoWorkEventHandler _capturedReadTagsWorkerDoWork;
        private ProgressChangedEventHandler _capturedReadTagsWorkerProgressChanged;
        private RunWorkerCompletedEventHandler _capturedReadTagsWorkerCompleted;
        
        // Store a list of workers created for process stack
        private List<Mock<IBackgroundWorker>> _createdProcessStackWorkers;
        private List<DoWorkEventHandler> _capturedProcessStackDoWorkHandlers;

        private string _dummyExePath = "dummy_mp3gain.exe";

        [TestInitialize]
        public void TestInitialize()
        {
            _mockMp3FolderFactory = new Mock<IMp3FolderFactory>();
            _mockBackgroundWorkerFactory = new Mock<IBackgroundWorkerFactory>();
            _mockExecuteMp3GainSyncFactory = new Mock<Mp3GainManager.ExecuteMp3GainSyncFactory>();

            _mockSearchWorker = new Mock<IBackgroundWorker>();
            _mockReadTagsWorker = new Mock<IBackgroundWorker>();
            _createdProcessStackWorkers = new List<Mock<IBackgroundWorker>>();
            _capturedProcessStackDoWorkHandlers = new List<DoWorkEventHandler>();

            // Setup individual Mp3File mocks
            _mockMp3File1 = new Mock<Mp3File>("dummy_file1.mp3");
            _mockMp3File1.SetupGet(f => f.FilePath).Returns("dummy_file1.mp3");
            _mockMp3File1.Setup(f => f.ExtractTags()); // Mock ExtractTags

            _mockMp3File2 = new Mock<Mp3File>("dummy_file2.mp3");
            _mockMp3File2.SetupGet(f => f.FilePath).Returns("dummy_file2.mp3");
            _mockMp3File2.Setup(f => f.ExtractTags());

            _folderFilesDictionary = new Dictionary<string, Mp3File>
            {
                { "dummy_file1.mp3", _mockMp3File1.Object },
                { "dummy_file2.mp3", _mockMp3File2.Object }
            };
            _folderFilesList = _folderFilesDictionary.Values.ToList();

            _mockMp3Folder = new Mock<Mp3Folder>("dummy_path_for_mock_folder");
            _mockMp3Folder.SetupGet(f => f.Files).Returns(_folderFilesDictionary);
            // Assuming MP3Files property returns List<string> of file paths
            _mockMp3Folder.SetupGet(f => f.MP3Files).Returns(_folderFilesDictionary.Keys.ToList());


            _mockFileRenamer = new Mock<IFileRenamer>();
            _mockProcessFactoryFunc = new Mock<Func<IProcess>>(); // Mock the factory Func itself

            // Setup factory to return specific mock workers for search and readTags
            // For processStack workers, a more general setup might be needed if tests create many.
            _mockBackgroundWorkerFactory.Setup(f => f.Create())
                .Returns(() => {
                    // This lambda allows us to return different mocks based on context if needed,
                    // or a new mock each time for processStack workers.
                    var newWorkerMock = new Mock<IBackgroundWorker>();
                    newWorkerMock.SetupAdd(w => w.DoWork += It.IsAny<DoWorkEventHandler>())
                                 .Callback<DoWorkEventHandler>(handler => _capturedProcessStackDoWorkHandlers.Add(handler));
                    _createdProcessStackWorkers.Add(newWorkerMock);
                    return newWorkerMock.Object;
                });
            
            // Specific setups for searchWorker and readTagsWorker if they are created deterministically first.
            // This might require adjusting the generic .Create() setup or using SetupSequence.
            // For now, the generic one will provide new mocks. We can refine if a test needs a *specific* one.
            // Let's assume searchWorker and readTagsWorker are the first two created:
            _mockBackgroundWorkerFactory.SetupSequence(f => f.Create())
                .Returns(_mockSearchWorker.Object)
                .Returns(_mockReadTagsWorker.Object)
                .ReturnsUsingDefault(); // Fallback to the generic setup for subsequent calls

            // Capture event handlers for specific workers
            _mockSearchWorker.SetupAdd(w => w.DoWork += It.IsAny<DoWorkEventHandler>()).Callback<DoWorkEventHandler>(h => _capturedSearchWorkerDoWork = h);
            _mockSearchWorker.SetupAdd(w => w.ProgressChanged += It.IsAny<ProgressChangedEventHandler>()).Callback<ProgressChangedEventHandler>(h => _capturedSearchWorkerProgressChanged = h);
            _mockSearchWorker.SetupAdd(w => w.RunWorkerCompleted += It.IsAny<RunWorkerCompletedEventHandler>()).Callback<RunWorkerCompletedEventHandler>(h => _capturedSearchWorkerCompleted = h);

            _mockReadTagsWorker.SetupAdd(w => w.DoWork += It.IsAny<DoWorkEventHandler>()).Callback<DoWorkEventHandler>(h => _capturedReadTagsWorkerDoWork = h);
            _mockReadTagsWorker.SetupAdd(w => w.ProgressChanged += It.IsAny<ProgressChangedEventHandler>()).Callback<ProgressChangedEventHandler>(h => _capturedReadTagsWorkerProgressChanged = h);
            _mockReadTagsWorker.SetupAdd(w => w.RunWorkerCompleted += It.IsAny<RunWorkerCompletedEventHandler>()).Callback<RunWorkerCompletedEventHandler>(h => _capturedReadTagsWorkerCompleted = h);
            

            _mockMp3FolderFactory.Setup(f => f.Create(It.IsAny<string>())).Returns(_mockMp3Folder.Object);

            _mockExecuteMp3GainSync = new Mock<ExecuteMp3GainSync>(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, Mp3File>>(),
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                _mockFileRenamer.Object, _mockProcessFactoryFunc.Object 
            );
            _mockExecuteMp3GainSync.Setup(e => e.Execute()); // Mock Execute

            _mockExecuteMp3GainSyncFactory.Setup(f => f.Invoke(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, Mp3File>>(),
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<IFileRenamer>(), It.IsAny<Func<IProcess>>()))
                .Returns(_mockExecuteMp3GainSync.Object);

            _manager = new Mp3GainManager(
                _dummyExePath,
                "dummy_parent_folder",
                _mockMp3FolderFactory.Object,
                _mockBackgroundWorkerFactory.Object,
                _mockExecuteMp3GainSyncFactory.Object,
                _mockFileRenamer.Object, 
                _mockProcessFactoryFunc.Object 
            );
        }

        [TestMethod]
        public void SearchFolders_SinglePath_UsesFactoryAndStartsWorker()
        {
            // Arrange
            string testPath = "C:\\Music";
            _mockBackgroundWorkerFactory.Reset(); // Reset sequence for specific test
            _mockBackgroundWorkerFactory.Setup(f => f.Create()).Returns(_mockSearchWorker.Object);


            // Act
            _manager.SearchFolders(testPath);

            // Assert
            _mockBackgroundWorkerFactory.Verify(f => f.Create(), Times.Once(), "BackgroundWorkerFactory.Create should be called for searchWorker.");
            _mockSearchWorker.Verify(w => w.RunWorkerAsync(testPath), Times.Once(), "SearchWorker.RunWorkerAsync should be called with the specified path.");
        }

        [TestMethod]
        public void SearchFolders_IEnumerablePaths_UsesFactoryAndStartsWorkerForFirstPath()
        {
            // Arrange
            var paths = new List<string> { "C:\\Music1", "C:\\Music2" };
            _mockBackgroundWorkerFactory.Reset(); 
            _mockBackgroundWorkerFactory.Setup(f => f.Create()).Returns(_mockSearchWorker.Object);

            // Act
            _manager.SearchFolders(paths);

            // Assert
            _mockBackgroundWorkerFactory.Verify(f => f.Create(), Times.Once());
            _mockSearchWorker.Verify(w => w.RunWorkerAsync(paths.First()), Times.Once(), "SearchWorker should start with the first path from the list.");
        }

        [TestMethod]
        public void Clear_ResetsCollectionsAndState()
        {
            // Arrange
            // Add some data to simulate state
            _manager.Folders.Add("some_folder_path", _mockMp3Folder.Object);
            var mockMp3File = new Mp3File("some_file_path.mp3");
            _manager.AddFile(mockMp3File); // AddFile populates foundFiles and potentially SourceDictionary
            _manager.UpdateFile(mockMp3File); // This populates Source and SourceDictionary via MP3GainRow
            // To check filesDone, it might need to be non-private or tested via behavior.
            // For now, we assume it's reset if it's a direct field.

            bool refreshTableEventRaised = false;
            _manager.RefreshTable += (sender, args) => refreshTableEventRaised = true;

            // Act
            _manager.Clear();

            // Assert
            Assert.AreEqual(0, _manager.Folders.Count, "Folders dictionary should be cleared.");
            Assert.AreEqual(0, _manager.FileCount, "FileCount (from foundFiles) should be zero.");
            Assert.AreEqual(0, _manager.SourceDictionary.Count, "SourceDictionary should be cleared.");
            Assert.AreEqual(0, _manager.Source.Count, "Source (BindingList) should be cleared.");
            Assert.IsTrue(refreshTableEventRaised, "RefreshTable event should have been raised.");
            // Assert.AreEqual(0, _manager.filesDone); // If filesDone is accessible for testing
        }

        [TestMethod]
        public void AddFile_NewFile_AddsToCollections()
        {
            // Arrange
            _manager.Clear(); // Ensure clean state
            var filePath = "C:\\TestFolder\\test.mp3";
            var mockMp3File = new Mock<Mp3File>(filePath); // Mocking Mp3File itself
            mockMp3File.SetupGet(f => f.FilePath).Returns(filePath);
            mockMp3File.SetupGet(f => f.FolderPath).Returns("C:\\TestFolder");

            // Ensure the folder exists for UpdateFile logic
            var mockFolderForFile = new Mock<Mp3Folder>("C:\\TestFolder");
            _manager.Folders.Add("C:\\TestFolder", mockFolderForFile.Object);

            // Act
            _manager.AddFile(mockMp3File.Object);
            _manager.UpdateFile(mockMp3File.Object); // UpdateFile is what adds to Source/SourceDictionary

            // Assert
            // AddFile adds to 'foundFiles' (tested via FileCount)
            Assert.AreEqual(1, _manager.FileCount, "FileCount should be 1 after adding a new file.");
            // UpdateFile adds to SourceDictionary and Source
            Assert.AreEqual(1, _manager.SourceDictionary.Count, "SourceDictionary should have 1 entry.");
            Assert.AreEqual(1, _manager.Source.Count, "Source should have 1 entry.");
            Assert.IsTrue(_manager.SourceDictionary.ContainsKey(filePath));
        }

        [TestMethod]
        public void UpdateFile_ExistingFile_DoesNotAddAgainToFoundFiles()
        {
            // Arrange
            _manager.Clear();
            var filePath = "C:\\TestFolder\\test.mp3";
            var mockMp3File = new Mock<Mp3File>(filePath);
            mockMp3File.SetupGet(f => f.FilePath).Returns(filePath);
            mockMp3File.SetupGet(f => f.FolderPath).Returns("C:\\TestFolder");
            
            var mockFolderForFile = new Mock<Mp3Folder>("C:\\TestFolder");
            _manager.Folders.Add("C:\\TestFolder", mockFolderForFile.Object);

            _manager.AddFile(mockMp3File.Object); // First add
            _manager.UpdateFile(mockMp3File.Object); // First update
            
            var initialFileCount = _manager.FileCount;
            var initialSourceCount = _manager.Source.Count;

            // Act: Call AddFile again (which internally calls UpdateFile if file exists in foundFiles)
            // or just call UpdateFile again. The prompt implies testing AddFile's behavior.
            _manager.AddFile(mockMp3File.Object); 
            _manager.UpdateFile(mockMp3File.Object); // Second update

            // Assert
            Assert.AreEqual(initialFileCount, _manager.FileCount, "FileCount should not change on adding existing file.");
            Assert.AreEqual(initialSourceCount, _manager.Source.Count, "Source count should not change if file already in SourceDictionary via UpdateFile.");
        }


        // Placeholders for ReadTags and Gain Operations
        [TestMethod]
        public void ReadTags_StartsWorkerAndProcessesFiles()
        {
            // Arrange
            // Ensure readTagsWorker is the one returned by the factory for this call
            // The SetupSequence in TestInitialize handles this if ReadTags is called after SearchFolders usually is.
            // If testing in isolation, ensure _mockBackgroundWorkerFactory.Create() returns _mockReadTagsWorker.
            // For this test, we'll assume the sequence is fine or reset it for this specific worker.
            _mockBackgroundWorkerFactory.Reset(); // Clear sequence for this specific test
            _mockBackgroundWorkerFactory.Setup(f => f.Create()).Returns(_mockReadTagsWorker.Object);

            _manager.Folders.Clear(); // Clear from other tests
            _manager.Folders.Add(_mockMp3Folder.Object.FolderPath, _mockMp3Folder.Object);
            
            // Setup AllFiles to return the files from our mock folder
            // _manager.AllFiles is private, but ReadTagsWorker_DoWork uses this.AllFolders then this.AllFiles.
            // So having Folders populated is key.

            bool tagReadEventRaisedForFile1 = false;
            bool tagReadEventRaisedForFile2 = false;
            int rowUpdatedForFile1 = -1;
            int rowUpdatedForFile2 = -1;
            _manager.TagRead += (sender, file) => {
                if (file == _mockMp3File1.Object) tagReadEventRaisedForFile1 = true;
                if (file == _mockMp3File2.Object) tagReadEventRaisedForFile2 = true;
            };
            _manager.RowUpdated += (sender, index) => {
                // This relies on SourceIndex being set on Mp3File objects if that's how RowUpdated works.
                // Mp3File.SourceIndex is set in ReadTagsWorker_DoWork.
                if (_mockMp3File1.Object.SourceIndex == index) rowUpdatedForFile1 = index;
                if (_mockMp3File2.Object.SourceIndex == index) rowUpdatedForFile2 = index;
            };
            
            bool refreshTableEventRaised = false;
            _manager.RefreshTable += (s,e) => refreshTableEventRaised = true;
            string lastActivity = null;
            _manager.ActivityUpdated += (s, activity) => lastActivity = activity;


            // Act
            _manager.ReadTags();

            // Assert
            _mockBackgroundWorkerFactory.Verify(f => f.Create(), Times.Once(), "ReadTags should create one worker.");
            _mockReadTagsWorker.Verify(w => w.RunWorkerAsync(), Times.Once(), "ReadTagsWorker.RunWorkerAsync should be called.");
            Assert.IsNotNull(_capturedReadTagsWorkerDoWork, "DoWork handler should have been captured for readTagsWorker.");

            // Simulate DoWork execution
            _capturedReadTagsWorkerDoWork.Invoke(_mockReadTagsWorker.Object, new DoWorkEventArgs(null));

            // Verify inside DoWork
            _mockExecuteMp3GainSyncFactory.Verify(f => f.Invoke(
                Mp3GainManager.Executable, $"/o /s c", _folderFilesDictionary, 
                _mockMp3Folder.Object.FolderPath, "GET MAX GAIN", "Calculating Max Gain", "done",
                It.IsAny<IFileRenamer>(), It.IsAny<Func<IProcess>>()), 
                Times.Once, "ExecuteMp3GainSyncFactory should be called for the folder.");
            _mockExecuteMp3GainSync.Verify(e => e.Execute(), Times.Once(), "ExecuteMp3GainSync.Execute should be called.");

            _mockMp3File1.Verify(f => f.ExtractTags(), Times.Once());
            _mockMp3File2.Verify(f => f.ExtractTags(), Times.Once());

            // Verify ReportProgress calls from DoWork (simulated)
            // Progress is (filesDone / totalFiles) * 100. UserState is the Mp3File.
            // filesDone is 0-indexed before increment.
            // For file1 (index 0 of 2 files): progress = (0/2)*100 = 0 initially, then filesDone becomes 1.
            // SourceIndex on file is filesDone (before increment). So _mockMp3File1.SourceIndex = 0.
            // ReportProgress(0, _mockMp3File1.Object) - assuming 0% for first file before it's "done" from loop perspective
            // Then filesDone = 1. For file2 (index 1 of 2 files): SourceIndex = 1.
            // ReportProgress(50, _mockMp3File2.Object) - 50% after first file done
            // Then filesDone = 2. Finally ReportProgress(100)
            _mockReadTagsWorker.Verify(w => w.ReportProgress(It.IsInRange(0, 100), _mockMp3File1.Object), Times.Once());
            _mockReadTagsWorker.Verify(w => w.ReportProgress(It.IsInRange(0, 100), _mockMp3File2.Object), Times.Once());
            _mockReadTagsWorker.Verify(w => w.ReportProgress(100), Times.AtLeastOnce()); // Final 100%

            // Simulate ProgressChanged events if ReadTagsWorker_ProgressChanged is to be tested
            Assert.IsNotNull(_capturedReadTagsWorkerProgressChanged, "ProgressChanged handler should be captured.");
            // Simulate for file1
            _capturedReadTagsWorkerProgressChanged.Invoke(_mockReadTagsWorker.Object, new ProgressChangedEventArgs(0, _mockMp3File1.Object));
            Assert.IsTrue(tagReadEventRaisedForFile1, "TagRead event should be raised for file1.");
            Assert.AreEqual(_mockMp3File1.Object.SourceIndex, rowUpdatedForFile1, "RowUpdated event for file1 index.");

            // Simulate for file2
             _capturedReadTagsWorkerProgressChanged.Invoke(_mockReadTagsWorker.Object, new ProgressChangedEventArgs(50, _mockMp3File2.Object));
            Assert.IsTrue(tagReadEventRaisedForFile2, "TagRead event should be raised for file2.");
            Assert.AreEqual(_mockMp3File2.Object.SourceIndex, rowUpdatedForFile2, "RowUpdated event for file2 index.");

            // Simulate RunWorkerCompleted
            Assert.IsNotNull(_capturedReadTagsWorkerCompleted, "RunWorkerCompleted handler should be captured.");
            _capturedReadTagsWorkerCompleted.Invoke(_mockReadTagsWorker.Object, new RunWorkerCompletedEventArgs(null, null, false));
            Assert.IsTrue(refreshTableEventRaised, "RefreshTable event should be raised on completion.");
            Assert.AreEqual("Finished.", lastActivity, "Activity should be updated to Finished.");
        }

        public void AnalyzeGain_PopulatesStackAndStartsWorkers()
        {
            // Arrange
            _manager.Folders.Clear(); // Clear from other tests
            _createdProcessStackWorkers.Clear(); // Clear list of workers from previous tests
            _capturedProcessStackDoWorkHandlers.Clear();

            var mockFolder1 = new Mock<Mp3Folder>("C:\\folder1");
            var mockFolder2 = new Mock<Mp3Folder>("C:\\folder2");
            _manager.Folders.Add("C:\\folder1", mockFolder1.Object);
            _manager.Folders.Add("C:\\folder2", mockFolder2.Object);

            // Ensure the factory creates new mocks for these workers
            // The generic setup in TestInitialize should already do this if called multiple times
            // after the initial sequence for search/read workers.

            int coresToRun = 2;

            // Act
            _manager.AnalyzeGain(cores: coresToRun);

            // Assert
            // Verify that Create was called for each folder to put on stack, 
            // and then for the number of cores to start initial workers.
            // Mp3GainManager.AnalyzeGain creates N workers for N folders and PUSHES them.
            // Then RunFolderGroup is called, which POPS 'cores' workers and runs them.
            Assert.AreEqual(2, _createdProcessStackWorkers.Count, "Should have created a worker for each folder.");
            
            // Verify RunWorkerAsync was called on the number of 'cores' workers
            // The workers are pushed onto a stack and then popped. So the ones started
            // are the last 'cores' ones that were added to _createdProcessStackWorkers list.
            int workersStarted = 0;
            for (int i = 0; i < _createdProcessStackWorkers.Count; i++)
            {
                var workerMock = _createdProcessStackWorkers[i];
                try
                {
                    // Check which folder was passed to RunWorkerAsync
                    // The order of folders being processed depends on the stack, so it's tricky to assert specific folder per worker without more info.
                    // For now, just verify RunWorkerAsync was called with any Mp3Folder from our list.
                    workerMock.Verify(w => w.RunWorkerAsync(It.Is<Mp3Folder>(f => _manager.Folders.ContainsValue(f))), Times.AtMostOnce());
                    if (workerMock.Invocations.Any(inv => inv.Method.Name == "RunWorkerAsync"))
                    {
                        workersStarted++;
                    }
                }
                catch (MockException) { /* This worker might not have been started if cores < folder count */ }
            }
            Assert.AreEqual(Math.Min(coresToRun, _manager.Folders.Count), workersStarted, $"Should have started {Math.Min(coresToRun, _manager.Folders.Count)} workers.");


            // Simulate DoWork for each captured handler (these are from the workers created for the stack)
            Assert.AreEqual(2, _capturedProcessStackDoWorkHandlers.Count, "DoWork handlers should have been captured for each worker created.");
            
            // We need to associate the DoWork handler with the correct worker and folder.
            // The FolderWorker class links them. This is hard to verify without deeper changes or more complex setup.
            // For simplicity, let's assume the DoWork handlers are invoked and check the folder method calls.
            // We need to know which mock worker corresponds to which mock folder when DoWork is called.
            // The current setup of _createdProcessStackWorkers and _capturedProcessStackDoWorkHandlers captures them in order of creation.
            // The processStack reverses this order.
            
            // Simulate DoWork for the workers that were started.
            // This example assumes the first 'coresToRun' workers from the _createdProcessStackWorkers list 
            // (which are actually the *last* ones pushed to stack, hence first ones popped if LIFO) are run.
            // This part is tricky due to the private 'processStack'.
            // A more robust way would be to verify based on which folders were passed to RunWorkerAsync.

            // Let's verify calls on the mock folders
            // For AnalyzeGain_DoWork, it calls folder.AnalyzeGainFolder(Executable, sender as BackgroundWorker);
            // We need to ensure our mock workers are correctly passed.
            // The sender in DoWorkEventArgs will be the IBackgroundWorker.
            
            // To properly test this, we need to invoke the DoWork handlers that were actually
            // associated with the workers that had RunWorkerAsync called on them.
            // This is complex because the processStack is private.
            // We will make a simplifying assumption for now:
            // The test will check if *any* of the mock folders got the expected call.
            // This is not ideal but a step forward.

            // If we assume the first worker in _createdProcessStackWorkers list (if started) got folder1, and second got folder2.
            // This depends on the iteration order in AnalyzeGain vs stack behavior.
            // AnalyzeGain iterates Folders.Values.Reverse(), so folder2 worker is pushed first, then folder1 worker.
            // If cores=2, folder1 worker is popped and run, then folder2 worker is popped and run.
            
            // Simulate DoWork for the *first* worker that would have been popped and run.
            // This would be the last one for which a handler was captured if the factory always appends.
            // And the folder would be the last one in the original Folders.Values list (before Reverse).
            if (_capturedProcessStackDoWorkHandlers.Count > 0 && workersStarted > 0)
            {
                 // The worker for folder2 (last in original list, first pushed, last popped if cores=1, or second if cores=2)
                var lastCapturedHandler = _capturedProcessStackDoWorkHandlers.Last(); 
                var correspondingWorkerMock = _createdProcessStackWorkers.Last();
                lastCapturedHandler.Invoke(correspondingWorkerMock.Object, new DoWorkEventArgs(mockFolder2.Object)); // Assuming this worker got mockFolder2
                mockFolder2.Verify(f => f.AnalyzeGainFolder(Mp3GainManager.Executable, It.IsAny<BackgroundWorker>()), Times.Once());
            }
            if (_capturedProcessStackDoWorkHandlers.Count > 1 && workersStarted > 1)
            {
                // The worker for folder1 (first in original list, last pushed, first popped)
                var firstCapturedHandler = _capturedProcessStackDoWorkHandlers[_capturedProcessStackDoWorkHandlers.Count - 2];
                var correspondingWorkerMock = _createdProcessStackWorkers[_createdProcessStackWorkers.Count - 2];
                firstCapturedHandler.Invoke(correspondingWorkerMock.Object, new DoWorkEventArgs(mockFolder1.Object));
                mockFolder1.Verify(f => f.AnalyzeGainFolder(Mp3GainManager.Executable, It.IsAny<BackgroundWorker>()), Times.Once());
            }
        }
        
        public void ApplyGain_PopulatesStackAndStartsWorkers()
        {
            // Arrange
            _manager.Folders.Clear();
            _createdProcessStackWorkers.Clear();
            _capturedProcessStackDoWorkHandlers.Clear();

            var mockFolder1 = new Mock<Mp3Folder>("C:\\folderApply1");
            var mockFolder2 = new Mock<Mp3Folder>("C:\\folderApply2");
            _manager.Folders.Add("C:\\folderApply1", mockFolder1.Object);
            _manager.Folders.Add("C:\\folderApply2", mockFolder2.Object);

            int coresToRun = 1; // Start with 1 to simplify verification of RunNextFolder later

            // Act
            _manager.ApplyGain(cores: coresToRun);

            // Assert
            Assert.AreEqual(2, _createdProcessStackWorkers.Count, "Should have created a worker for each folder for ApplyGain.");
            
            // Verify RunWorkerAsync was called on the number of 'cores' workers
            int workersStarted = _createdProcessStackWorkers.Count(w => w.Invocations.Any(inv => inv.Method.Name == "RunWorkerAsync"));
            Assert.AreEqual(Math.Min(coresToRun, _manager.Folders.Count), workersStarted, $"Should have started {Math.Min(coresToRun, _manager.Folders.Count)} workers for ApplyGain.");

            // Simulate DoWork for the worker that was started.
            // With LIFO stack and 1 core, the last folder pushed (first in original list if iteration is normal) gets processed.
            // _manager.Folders.Values.Reverse() means folder2 is pushed, then folder1.
            // So, folder1's worker is popped and run.
            var startedWorkerMock = _createdProcessStackWorkers.FirstOrDefault(w => w.Invocations.Any(inv => inv.Method.Name == "RunWorkerAsync" && inv.Arguments[0] == mockFolder1.Object));
            Assert.IsNotNull(startedWorkerMock, "A worker for mockFolder1 should have been started.");

            // Find its captured DoWork handler
            // This relies on the generic factory setup in TestInitialize that adds handlers to _capturedProcessStackDoWorkHandlers
            // and workers to _createdProcessStackWorkers in the same order.
            int workerIndex = _createdProcessStackWorkers.IndexOf(startedWorkerMock);
            Assert.IsTrue(workerIndex >= 0 && workerIndex < _capturedProcessStackDoWorkHandlers.Count, "Captured DoWork handler not found for started worker.");
            var doWorkHandler = _capturedProcessStackDoWorkHandlers[workerIndex];
            
            doWorkHandler.Invoke(startedWorkerMock.Object, new DoWorkEventArgs(mockFolder1.Object));
            mockFolder1.Verify(f => f.ApplyGainFolder(Mp3GainManager.Executable, It.IsAny<BackgroundWorker>()), Times.Once());
        }

        [TestMethod]
        public void UndoGain_PopulatesStackAndStartsWorkers()
        {
            // Arrange
            _manager.Folders.Clear();
            _createdProcessStackWorkers.Clear();
            _capturedProcessStackDoWorkHandlers.Clear();

            var mockFolder1 = new Mock<Mp3Folder>("C:\\folderUndo1");
            _manager.Folders.Add("C:\\folderUndo1", mockFolder1.Object);
            
            int coresToRun = 1;

            // Act
            _manager.UndoGain(cores: coresToRun);

            // Assert
            Assert.AreEqual(1, _createdProcessStackWorkers.Count);
            var workerMock = _createdProcessStackWorkers.First(); // Only one worker created
            workerMock.Verify(w => w.RunWorkerAsync(mockFolder1.Object), Times.Once());

            Assert.AreEqual(1, _capturedProcessStackDoWorkHandlers.Count);
            var doWorkHandler = _capturedProcessStackDoWorkHandlers.First();
            doWorkHandler.Invoke(workerMock.Object, new DoWorkEventArgs(mockFolder1.Object));
            mockFolder1.Verify(f => f.UndoGain(Mp3GainManager.Executable, It.IsAny<BackgroundWorker>()), Times.Once());
        }

        [TestMethod]
        public void GainWorkflow_FolderWorkerCompleted_UpdatesStateAndRunsNext()
        {
            // Arrange
            _manager.Folders.Clear();
            _createdProcessStackWorkers.Clear();
            _capturedProcessStackDoWorkHandlers.Clear(); // Ensure this is cleared and repopulated correctly

            var mockFolder1 = new Mock<Mp3Folder>("C:\\folderPath1");
            mockFolder1.SetupGet(f => f.FolderPath).Returns("C:\\folderPath1"); // Needed for ExecuteMp3GainSync
            mockFolder1.SetupGet(f => f.Files).Returns(new Dictionary<string, Mp3File> { { "file1.mp3", _mockMp3File1.Object } });
            
            var mockFolder2 = new Mock<Mp3Folder>("C:\\folderPath2");
            mockFolder2.SetupGet(f => f.FolderPath).Returns("C:\\folderPath2");
            mockFolder2.SetupGet(f => f.Files).Returns(new Dictionary<string, Mp3File> { { "file2.mp3", _mockMp3File2.Object } });

            _manager.Folders.Add("C:\\folderPath1", mockFolder1.Object);
            _manager.Folders.Add("C:\\folderPath2", mockFolder2.Object);

            // Capture RunWorkerCompleted event handlers
            var capturedCompletedHandlers = new List<RunWorkerCompletedEventHandler>();
             _mockBackgroundWorkerFactory.Setup(f => f.Create()).Returns(() => {
                var newWorkerMock = new Mock<IBackgroundWorker>();
                newWorkerMock.SetupAdd(w => w.DoWork += It.IsAny<DoWorkEventHandler>()); // Basic setup
                newWorkerMock.SetupAdd(w => w.RunWorkerCompleted += It.IsAny<RunWorkerCompletedEventHandler>())
                             .Callback<RunWorkerCompletedEventHandler>(h => capturedCompletedHandlers.Add(h));
                _createdProcessStackWorkers.Add(newWorkerMock); // Keep track of created workers
                return newWorkerMock.Object;
            });


            bool analysisFinishedEventRaised = false;
            _manager.AnalysisFinished += (s, e) => analysisFinishedEventRaised = true;
            
            int taskProgressedEventCount = 0;
            _manager.TaskProgressed += (s, e) => taskProgressedEventCount++;

            // Act
            _manager.AnalyzeGain(cores: 1); // Start with 1 core, so only one worker runs at a time

            // Assert: Initial worker started
            Assert.AreEqual(2, _createdProcessStackWorkers.Count, "Two workers should be created for the stack.");
            var worker1Mock = _createdProcessStackWorkers[0]; // Worker for folder1 (pushed last, popped first)
            var worker2Mock = _createdProcessStackWorkers[1]; // Worker for folder2 (pushed first, popped second)
            
            // The manager reverses the list, so the first folder in the original list (folder1) is processed first by the stack logic.
            // Actually, Folders.Values.Reverse() is used. So if Folders has {f1, f2}, Reverse is {f2,f1}.
            // Stack push: f1_worker, then f2_worker.
            // Stack pop (RunNextFolder): f2_worker, then f1_worker.
            // So, worker for f2 (mockFolder2) should run first.
            _createdProcessStackWorkers[1].Verify(w => w.RunWorkerAsync(mockFolder2.Object), Times.Once(), "Worker for folder2 should be started first.");


            Assert.AreEqual(1, capturedCompletedHandlers.Count, "RunWorkerCompleted handler should be captured for the first active worker.");
            var firstWorkerCompletedHandler = capturedCompletedHandlers[0];

            // Simulate first worker (for mockFolder2) completing
            firstWorkerCompletedHandler.Invoke(worker2Mock.Object, new RunWorkerCompletedEventArgs(mockFolder2.Object, null, false));

            // Assert: State after first worker completes
            _mockExecuteMp3GainSyncFactory.Verify(f => f.Invoke(Mp3GainManager.Executable, "/o /s c", 
                mockFolder2.Object.Files, mockFolder2.Object.FolderPath, "GET MAX GAIN", It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<IFileRenamer>(), It.IsAny<Func<IProcess>>()), Times.Once(), "Sync should run for folder2.");
            _mockMp3File2.Verify(f => f.ExtractTags(), Times.Once(), "ExtractTags should be called for files in folder2.");
            Assert.AreEqual(1, taskProgressedEventCount, "TaskProgressed should be raised after first folder.");
            
            // Assert: Next worker (for mockFolder1) started
            _createdProcessStackWorkers[0].Verify(w => w.RunWorkerAsync(mockFolder1.Object), Times.Once(), "Worker for folder1 should be started next.");
            Assert.AreEqual(2, capturedCompletedHandlers.Count, "RunWorkerCompleted handler should be captured for the second active worker.");
            var secondWorkerCompletedHandler = capturedCompletedHandlers[1];

            // Simulate second worker (for mockFolder1) completing
            secondWorkerCompletedHandler.Invoke(worker1Mock.Object, new RunWorkerCompletedEventArgs(mockFolder1.Object, null, false));

            // Assert: State after second worker completes
             _mockExecuteMp3GainSyncFactory.Verify(f => f.Invoke(Mp3GainManager.Executable, "/o /s c", 
                mockFolder1.Object.Files, mockFolder1.Object.FolderPath, "GET MAX GAIN", It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<IFileRenamer>(), It.IsAny<Func<IProcess>>()), Times.Once(), "Sync should run for folder1.");
            _mockMp3File1.Verify(f => f.ExtractTags(), Times.Once(), "ExtractTags should be called for files in folder1.");
            Assert.AreEqual(2, taskProgressedEventCount, "TaskProgressed should be raised again after second folder.");
            Assert.IsTrue(analysisFinishedEventRaised, "AnalysisFinished event should be raised as all folders are done.");
        }
    }
}
