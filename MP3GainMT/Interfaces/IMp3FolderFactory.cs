using MP3GainMT.MP3Gain; // For Mp3Folder

namespace MP3GainMT.Interfaces
{
    public interface IMp3FolderFactory
    {
        Mp3Folder Create(string path);
        // If the Mp3Folder constructor that takes factories is to be used by the manager,
        // this factory interface might need to pass those along too.
        // For now, assuming it uses the basic public constructor of Mp3Folder,
        // which itself chains to provide default factories for its dependencies.
    }
}
