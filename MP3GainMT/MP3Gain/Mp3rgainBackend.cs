using System.Linq;

namespace MP3GainMT.MP3Gain
{
    public class Mp3rgainBackend : OriginalMp3GainBackend
    {
        public override string Name => "mp3rgain (M-Igashi)";
        public override BackendSchedulingStrategy SchedulingStrategy => BackendSchedulingStrategy.TwoPhaseDynamic;

        protected override string ArgFormat => "-o tsv ";
        protected override string ArgApply => "-g ";
        protected override string ArgCheck => "-s c ";
        protected override string ArgUndo => "-u ";

        protected override string GetExtraArgs(int threadCount)
        {
            if (threadCount > 1)
            {
                return $"-j {threadCount}";
            }
            return "-j 1";
        }

        public Mp3rgainBackend(string executablePath) : base(executablePath)
        {
        }

        public override void ApplyGain(Mp3Folder folder, System.ComponentModel.BackgroundWorker worker, int threadCount = 1)
        {
            var applyGainExecution = new ExecuteMp3RGainAsync(this.ExecutablePath,
                                                        $"{ArgApply}{folder.SuggestedGain} {GetExtraArgs(threadCount)}".Trim(),
                                                        folder.Files,
                                                        folder.FolderPath,
                                                        "APPLY GAIN",
                                                        worker,
                                                        "Applying Track Gain",
                                                        "done");
            applyGainExecution.Execute();
        }

        public override void UndoGain(Mp3Folder folder, System.ComponentModel.BackgroundWorker worker, int threadCount = 1)
        {
            int undoSuggestedGain = -folder.SuggestedGain;
            if (folder.Files.Values.Any())
            {
                var firstFile = folder.Files.Values.First();
                undoSuggestedGain = -System.Convert.ToInt32(firstFile.GainUndoAlbum);
            }

            var undoGainExecution = new ExecuteMp3RGainAsync(this.ExecutablePath,
                                                        $"{ArgUndo}{GetExtraArgs(threadCount)}".Trim(),
                                                        folder.Files,
                                                        folder.FolderPath,
                                                        "UNDO GAIN",
                                                        worker,
                                                        "Undoing Track Gain",
                                                        "done");
            undoGainExecution.Execute();
        }

        public override void CalculateMaxGain(Mp3Folder folder, System.ComponentModel.BackgroundWorker worker, int threadCount = 1)
        {
            var execute = new ExecuteMp3RGainSync(this.ExecutablePath,
                                             $"{ArgFormat}{ArgCheck}{GetExtraArgs(threadCount)}".Trim(),
                                             folder.Files,
                                             folder.FolderPath,
                                             "GET MAX GAIN",
                                             "Calculating Max Gain",
                                             "done");
            execute.Execute();
        }
    }
}
