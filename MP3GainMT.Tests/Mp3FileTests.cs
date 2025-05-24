using Microsoft.VisualStudio.TestTools.UnitTesting;
using MP3GainMT.MP3Gain;
using Moq;
using System;
using System.IO;
using TagLib; // Used for TagLib.File, TagTypes, etc.
using TagLib.Ape; // Used for TagLib.Ape.Tag
using TagLib.Id3v2; // Used for TagLib.Id3v2.Tag

namespace MP3GainMT.Tests
{
    [TestClass]
    public class Mp3FileTests
    {
        private Mock<TagLib.File> _mockTagLibFile;
        private Mock<TagLib.Ape.Tag> _mockApeTag;
        private Mock<TagLib.Id3v2.Tag> _mockId3v2Tag;
        private Mock<TagLib.Tag> _mockGenericTag; // For common tag properties
        private Mp3File _mp3File;
        private string _dummyFilePath = "dummy.mp3";

        // Our mock factory for TagLib.File
        private Func<string, TagLib.File> _mockFileCreator;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockTagLibFile = new Mock<TagLib.File>();
            _mockApeTag = new Mock<TagLib.Ape.Tag>();
            _mockId3v2Tag = new Mock<TagLib.Id3v2.Tag>();
            _mockGenericTag = new Mock<TagLib.Tag>();

            // Default setup for the generic tag part of _mockTagLibFile
            _mockTagLibFile.Setup(f => f.Tag).Returns(_mockGenericTag.Object);
            _mockGenericTag.Setup(t => t.Performers).Returns(new string[0]);
            _mockGenericTag.Setup(t => t.Album).Returns("Test Album");
            _mockGenericTag.Setup(t => t.Title).Returns("Test Title");
            _mockGenericTag.Setup(t => t.Track).Returns((uint)1);
            _mockGenericTag.Setup(t => t.FirstAlbumArtist).Returns("Test Album Artist");

            // Default file creator is our mock
            _mockFileCreator = filePath => _mockTagLibFile.Object;

            // Mp3File instance is created using the mock factory
            _mp3File = new Mp3File(_dummyFilePath, _mockFileCreator);
            _mp3File.HasTags = false; // Ensure HasTags is false before each ExtractTags test
        }

        private void SetupApeTagItem(string key, string value)
        {
            // Ape.Item constructor takes (string key, string value) or (string key, params string[] values)
            // TagLib's Ape.Tag.GetItem returns an Ape.Item. ToString() on Ape.Item returns the value.
            var apeItem = new Ape.Item(key, value);
            _mockApeTag.Setup(t => t.HasItem(key)).Returns(true);
            _mockApeTag.Setup(t => t.GetItem(key)).Returns(apeItem);
        }

        [TestMethod]
        public void ExtractTags_ValidApeTags_PopulatesProperties()
        {
            // Arrange
            _mockTagLibFile.Setup(f => f.GetTag(TagTypes.Ape, false)).Returns(_mockApeTag.Object);
            _mockTagLibFile.Setup(f => f.GetTag(TagTypes.Id3v2, false)).Returns((TagLib.Id3v2.Tag)null); // No ID3v2 for this specific case

            SetupApeTagItem(Mp3File.TagMp3GainUndo, "1,2,MODIFIED_LABEL"); // TrackGainUndo, AlbumGainUndo, Label
            SetupApeTagItem(Mp3File.TagReplayAlbumGain, "3.0 dB");
            SetupApeTagItem(Mp3File.TagReplayAlbumPeak, "0.9");
            SetupApeTagItem(Mp3File.TagReplayTrackGain, "1.5 dB");
            SetupApeTagItem(Mp3File.TagReplayTrackPeak, "0.8");

            // Act
            _mp3File.ExtractTags();

            // Assert
            Assert.IsTrue(_mp3File.HasGainTags, "HasGainTags should be true when all ReplayGain APE tags are present.");
            Assert.AreEqual(1.5, _mp3File.ReplayTrackGain, 0.001, "ReplayTrackGain is incorrect.");
            Assert.AreEqual(3.0, _mp3File.ReplayAlbumGain, 0.001, "ReplayAlbumGain is incorrect.");
            Assert.AreEqual(0.8, _mp3File.ReplayTrackPeak, 0.001, "ReplayTrackPeak is incorrect.");
            Assert.AreEqual(0.9, _mp3File.ReplayAlbumPeak, 0.001, "ReplayAlbumPeak is incorrect.");
            Assert.AreEqual(1, _mp3File.GainUndoTrack, 0.001, "GainUndoTrack is incorrect.");
            Assert.AreEqual(2, _mp3File.GainUndoAlbum, 0.001, "GainUndoAlbum is incorrect.");
            Assert.AreEqual("MODIFIED_LABEL", _mp3File.GainUndoLabel, "GainUndoLabel is incorrect.");
            Assert.AreEqual(0, _mp3File.ErrorMessages.Count, "There should be no error messages.");
            Assert.IsTrue(_mp3File.HasTags, "HasTags should be true after successful extraction.");
            Assert.AreEqual("Test Album", _mp3File.Album);
            Assert.AreEqual("Test Title", _mp3File.Title);
        }

        [TestMethod]
        public void ExtractTags_CorruptFileException_AddsErrorMessage()
        {
            // Arrange
            _mockFileCreator = filePath => throw new CorruptFileException("Test corruption");
            _mp3File = new Mp3File(_dummyFilePath, _mockFileCreator); // Recreate with factory that throws
            _mp3File.HasTags = false;

            // Act
            _mp3File.ExtractTags();

            // Assert
            Assert.IsTrue(_mp3File.HasErrors, "HasErrors should be true.");
            Assert.AreEqual(1, _mp3File.ErrorMessages.Count, "Should have one error message.");
            Assert.IsTrue(_mp3File.ErrorMessages[0].Contains("Tag Extract Error"), "Error message header is wrong.");
            Assert.IsTrue(_mp3File.ErrorMessages[0].Contains("Test corruption"), "Error message content is wrong.");
            Assert.IsFalse(_mp3File.HasGainTags, "HasGainTags should be false when an exception occurs.");
            Assert.IsFalse(_mp3File.HasTags, "HasTags should remain false.");
        }

        [TestMethod]
        public void ExtractTags_IOException_AddsErrorMessage()
        {
            // Arrange
            _mockFileCreator = filePath => throw new IOException("Test IO error");
            _mp3File = new Mp3File(_dummyFilePath, _mockFileCreator); // Recreate with factory that throws
            _mp3File.HasTags = false;

            // Act
            _mp3File.ExtractTags();

            // Assert
            Assert.IsTrue(_mp3File.HasErrors, "HasErrors should be true.");
            Assert.AreEqual(1, _mp3File.ErrorMessages.Count, "Should have one error message.");
            Assert.IsTrue(_mp3File.ErrorMessages[0].Contains("Still open in mp3gain Error"), "Error message header is wrong.");
            Assert.IsTrue(_mp3File.ErrorMessages[0].Contains("Test IO error"), "Error message content is wrong.");
            Assert.IsFalse(_mp3File.HasGainTags, "HasGainTags should be false when an exception occurs.");
            Assert.IsFalse(_mp3File.HasTags, "HasTags should remain false.");
        }

        [TestMethod]
        public void ExtractTags_Id3v2TagsOnly_NoApeTags_SetsHasGainTagsFalseAndAddsError()
        {
            // Arrange
            _mockTagLibFile.Setup(f => f.GetTag(TagTypes.Ape, false)).Returns((TagLib.Ape.Tag)null); // No APE tags
            _mockTagLibFile.Setup(f => f.GetTag(TagTypes.Id3v2, false)).Returns(_mockId3v2Tag.Object); // Has ID3v2 tags
            // Ensure the main Tag property is still set for other metadata
            _mockTagLibFile.Setup(f => f.Tag).Returns(_mockGenericTag.Object);


            // Act
            _mp3File.ExtractTags();

            // Assert
            Assert.IsFalse(_mp3File.HasGainTags, "HasGainTags should be false if only ID3v2 tags (no APE gain tags) are present.");
            Assert.IsTrue(_mp3File.HasErrors, "Should have an error message indicating no gain info.");
            Assert.AreEqual(1, _mp3File.ErrorMessages.Count, "Error message count is wrong.");
            Assert.AreEqual("File does not contain gain information.", _mp3File.ErrorMessages[0], "Error message content is wrong.");
            Assert.IsTrue(_mp3File.HasTags, "HasTags should be true as other metadata (album, title) was extracted.");
            Assert.AreEqual("Test Album", _mp3File.Album); // Verify other tags still processed
        }

        // --- IsFileLocked Tests ---
        private string _testLockFilePath;

        // Separate TestInitialize for file operations to avoid conflicts with Moq setup
        // However, MSTest runs all TestInitialize methods before each test.
        // Let's rename this or ensure it's only relevant to IsFileLocked tests.
        // For simplicity, we'll use a combined setup and ensure file paths are unique.
        [TestInitialize]
        public void InitializeFileForLockTests() // This will run after the main TestInitialize
        {
            _testLockFilePath = Path.Combine(Path.GetTempPath(), "MP3GainMTTests", Guid.NewGuid().ToString() + ".mp3");
            Directory.CreateDirectory(Path.GetDirectoryName(_testLockFilePath)); // Ensure directory exists
        }
        
        [TestCleanup]
        public void CleanupFileForLockTests() // This will run after each test
        {
            if (System.IO.File.Exists(_testLockFilePath))
            {
                try { System.IO.File.Delete(_testLockFilePath); } catch { /* best effort */ }
            }
        }

        [TestMethod]
        public void IsFileLocked_WhenFileIsNotLocked_ReturnsFalse()
        {
            // Arrange
            System.IO.File.WriteAllText(_testLockFilePath, "dummy content"); 
            var mp3FileWithRealFile = new Mp3File(_testLockFilePath); // Uses default TagLib.File.Create

            // Act
            bool isLocked = mp3FileWithRealFile.IsFileLocked();

            // Assert
            Assert.IsFalse(isLocked, "File should not be locked.");
        }

        [TestMethod]
        public void IsFileLocked_WhenFileIsLocked_ReturnsTrue()
        {
            // Arrange
            System.IO.File.WriteAllText(_testLockFilePath, "dummy content");
            var mp3FileWithRealFile = new Mp3File(_testLockFilePath);

            // Act & Assert
            using (FileStream stream = new FileStream(_testLockFilePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
            {
                bool isLocked = mp3FileWithRealFile.IsFileLocked();
                Assert.IsTrue(isLocked, "File should be reported as locked.");
                stream.Close(); 
            }
        }
        
        [TestMethod]
        public void IsFileLocked_WhenFileDoesNotExist_ReturnsTrue()
        {
            // Arrange
            // Ensure file does not exist by using a new GUID path not yet written to.
            string nonExistentFilePath = Path.Combine(Path.GetTempPath(), "MP3GainMTTests", Guid.NewGuid().ToString() + ".mp3");
            var mp3FileWithNonExistentFile = new Mp3File(nonExistentFilePath);

            // Act
            bool isLocked = mp3FileWithNonExistentFile.IsFileLocked();

            // Assert
            Assert.IsTrue(isLocked, "Non-existent file should be reported as locked/inaccessible by IsFileLocked's logic due to IOException.");
        }
    }
}
