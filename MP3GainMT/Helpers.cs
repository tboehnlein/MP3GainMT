using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace MP3GainMT
{
    public static class Helpers
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
            return Convert.ToInt32(((double)(completedCount) / (double)totalCount) * 100.0);
        }

        public static void UpdateProgressTick(string activity, Label label, int maxPeriodCount)
        {
            if (activity.EndsWith("]"))
            {
                label.Text = label.Text.Insert(label.Text.Length - 1, ".");

                if (label.Text.EndsWith(new string('.', maxPeriodCount + 1) + "]"))
                {
                    label.Text = activity;
                }
            }
            else
            {
                label.Text += ".";

                if (label.Text.EndsWith(new string('.', maxPeriodCount + 1)))
                {
                    label.Text = activity;
                }
            }

            label.Invalidate();
            label.Refresh();
        }
    }
}