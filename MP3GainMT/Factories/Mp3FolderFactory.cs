using MP3GainMT.Interfaces;
using MP3GainMT.MP3Gain;

namespace MP3GainMT.Factories
{
    public class Mp3FolderFactory : IMp3FolderFactory
    {
        // This factory will use the public constructor of Mp3Folder.
        // That public constructor has been refactored to accept its own dependencies' factories
        // and provide default concrete implementations for them (FileRenamer, ProcessWrapper factory).
        public Mp3Folder Create(string path)
        {
            return new Mp3Folder(path);
        }
    }
}
