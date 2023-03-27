using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MP3GainMT
{
    internal static class Helpers
    {
        public static string AsSingleLine(this List<string> list)
        {
            var line = string.Empty;

            foreach (var item in list)
            {
                line += $"{item.ToString()};";
            }

            line = line.TrimEnd(';');

            return line;    
        }

        public static int GetProgress(int completedCount, int totalCount)
        {
            return Convert.ToInt32(((double)(completedCount) / (double) totalCount) * 100.0);
        }
    }
}
