namespace MP3GainMT
{
    internal class TaskUpdate
    {
        public TaskUpdate(int progressPercent, string message, int index)
        {
            ProgressPercent = progressPercent;
            Message = message;
            Index = index;
        }

        public TaskUpdate(int progressPercent, int index)
        {
            ProgressPercent = progressPercent;
            Message = string.Empty;
            Index = index;
        }

        public TaskUpdate(int progressPercent)
        {
            ProgressPercent = progressPercent;
            Message = string.Empty;
            Index = -1;
        }

        public TaskUpdate(int completedCount, int totalCount, string message)
        {
            ProgressPercent = Helpers.GetProgress(completedCount, totalCount);
            Message = message;
            Index = completedCount - 1;
        }

        public int ProgressPercent { get; set; }
        public string Message { get; set; }
        public int Index { get; set; }
    }
}