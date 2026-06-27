namespace MP3GainMT.MP3Gain
{
    public class Mp3gainModernBackend : OriginalMp3GainBackend
    {
        public override string Name => "mp3gain-modern (Multi-threaded)";

        public Mp3gainModernBackend(string executablePath) : base(executablePath)
        {
        }
    }
}
