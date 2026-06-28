namespace MP3GainMT.MP3Gain
{
    public class Mp3rgainBackend : OriginalMp3GainBackend
    {
        public override string Name => "mp3rgain (M-Igashi)";
        public override BackendSchedulingStrategy SchedulingStrategy => BackendSchedulingStrategy.TwoPhaseDynamic;

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
    }
}
