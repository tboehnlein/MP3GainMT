using System;
using System.Collections.Generic;
using System.IO;
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

        public static bool UndoRandomlyRenameFiles(Dictionary<string, string> fileLookUp)
        {
            bool success = true;

            foreach (var renameFile in fileLookUp)
            {
                try
                {
                    File.Move(renameFile.Key, renameFile.Value);
                }
                catch (IOException ex)
                {
                    // Handle IO exceptions, such as file access issues
                    Console.WriteLine($"IO Exception for {renameFile.Value} while undoing the file rename: {ex.Message}");
                    success = false;
                }
                catch (UnauthorizedAccessException ex)
                {
                    // Handle unauthorized access exceptions
                    Console.WriteLine($"Unauthorized Access Exception for {renameFile.Value} while undoing the file rename: {ex.Message}");
                    success = false;
                }
                catch (Exception ex)
                {
                    // Handle any other exceptions
                    Console.WriteLine($"Exception for {renameFile.Value} while undoing the file rename: {ex.Message}");
                    success = false;
                }

                if (!success)
                {
                    break;
                }
            }

            if (success)
            {
                File.Delete("random_file.txt");
            }

            return success;
        }

        public static bool RandomlyRenameFiles(Dictionary<string, string> fileLookUp, List<string> originalFiles)
        {
            bool success = true;

            Random random = new Random();

            using (StreamWriter writer = new StreamWriter("random_file.txt"))
            {
                foreach (var file in originalFiles)
                {
                    try
                    {
                        var name = $"{originalFiles.IndexOf(file).ToString("00000")}_{random.Next(1000)}.mp3";
                        var finalName = Path.Combine(Path.GetDirectoryName(file), name);

                        // Write to file
                        writer.WriteLine($"{file} -> {finalName}");
                        writer.Flush();

                        // Rename file in the same folder
                        File.Move(file, finalName);

                        fileLookUp.Add(finalName, file);
                        
                    }
                    catch (IOException ex)
                    {
                        // Handle IO exceptions, such as file access issues
                        Console.WriteLine($"IO Exception for {file} while applying the file rename: {ex.Message}");
                        success = false;
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        // Handle unauthorized access exceptions
                        Console.WriteLine($"Unauthorized Access Exception for {file} while applying the file rename: {ex.Message}");
                        success = false;
                    }
                    catch (Exception ex)
                    {
                        // Handle any other exceptions
                        Console.WriteLine($"Exception for {file} while applying the file rename: {ex.Message}");
                        success = false;
                    }

                    if (!success)
                    {
                        break;
                    }
                }
            }

            return success;
        }
    }
}