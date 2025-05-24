// Copyright (c) 2025 Thomas Boehnlein
// 
// This software is provided 'as-is', without any express or implied
// warranty. In no event will the authors be held liable for any damages
// arising from the use of this software.
// 
// Permission is granted to anyone to use this software for any purpose,
// including commercial applications, and to alter it and redistribute it
// freely, subject to the following restrictions:
// 
// 1. The origin of this software must not be misrepresented; you must not
//    claim that you wrote the original software. If you use this software
//    in a product, an acknowledgment in the product documentation would be
//    appreciated but is not required.
// 2. Altered source versions must be plainly marked as such, and must not be
//    misrepresented as being the original software.
// 3. This notice may not be removed or altered from any source distribution.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;

namespace MP3GainMT.Helpers
{
    public static class Helper
    {
        // public static string BackupRandomFileName { get; private set; } = "random_file.txt"; // Moved to FileRenamer
        public static readonly string PlayFileChoice = "Play File";
        public static readonly string OpenFolderChoice = "Open Folder";

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