using System.Collections.Generic;

namespace MP3GainMT.Helpers
{
    public interface IFileRenamer
    {
        bool RandomlyRenameFiles(Dictionary<string, string> fileLookUp, List<string> sortedFiles);
        void UndoRandomlyRenameFiles(Dictionary<string, string> fileLookUp);
    }
}
