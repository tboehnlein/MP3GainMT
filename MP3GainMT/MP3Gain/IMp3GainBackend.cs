using System.ComponentModel;

namespace MP3GainMT.MP3Gain
{
    public interface IMp3GainBackend
    {
        string Name { get; }
        string ExecutablePath { get; set; }

        void AnalyzeGain(Mp3Folder folder, BackgroundWorker worker);
        void ApplyGain(Mp3Folder folder, BackgroundWorker worker);
        void UndoGain(Mp3Folder folder, BackgroundWorker worker);
        void CalculateMaxGain(Mp3Folder folder, BackgroundWorker worker);
    }
}
