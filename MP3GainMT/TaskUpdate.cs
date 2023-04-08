using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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
