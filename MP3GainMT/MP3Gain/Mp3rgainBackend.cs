namespace MP3GainMT.MP3Gain
{
    public class Mp3rgainBackend : OriginalMp3GainBackend
    {
        public override string Name => "mp3rgain (M-Igashi)";

        public Mp3rgainBackend(string executablePath) : base(executablePath)
        {
        }
    }
}
