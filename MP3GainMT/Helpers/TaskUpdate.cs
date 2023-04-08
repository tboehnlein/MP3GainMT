namespace MP3GainMT
{
    public class TaskUpdate
    {
        public TaskUpdate(int progressPercent, string message, int index)
        {
            ProgressPercent = progressPercent;
            Message = message;
            Index = index;
        }

        public int Index { get; set; }
        public string Message { get; set; }
        public int ProgressPercent { get; set; }
    }
}