using System.ComponentModel;

namespace MP3GainMT.MP3Gain
{
    public enum BackendSchedulingStrategy
    {
        StandardConcurrent,
        TwoPhaseDynamic,
        StrictlySequential
    }

    public interface IMp3GainBackend
    {
        string Name { get; }
        string ExecutablePath { get; set; }
        BackendSchedulingStrategy SchedulingStrategy { get; }

        void AnalyzeGain(Mp3Folder folder, BackgroundWorker worker, int threadCount = 1);
        void ApplyGain(Mp3Folder folder, BackgroundWorker worker, int threadCount = 1);
        void UndoGain(Mp3Folder folder, BackgroundWorker worker, int threadCount = 1);
        void CalculateMaxGain(Mp3Folder folder, BackgroundWorker worker, int threadCount = 1);
    }
}
