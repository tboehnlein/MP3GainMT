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

namespace MP3GainMT
{
    public static class Helpers
    {
        public static string BackupRandomFileName { get; private set; } = "random_file.txt";
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

        public static bool UndoRandomlyRenameFiles(Dictionary<string, string> fileLookUp)
        {
            bool success = true;

            if (fileLookUp.Count == 0)
            {
                return success;
            }

            string randomFilePath = Path.Combine(fileLookUp.Values.First(), Helpers.BackupRandomFileName);

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
                if (File.Exists(randomFilePath))
                {
                    File.Delete(randomFilePath);
                }
            }

            return success;
        }

        public static bool UndoFileRenamesFromTextFile(string folderPath)
        {
            bool success = true;
            string randomTextFilePath = Path.Combine(folderPath, Helpers.BackupRandomFileName);

            if (File.Exists(randomTextFilePath))
            {
                var lines = File.ReadAllLines(randomTextFilePath);
                foreach (var line in lines)
                {
                    var parts = line.Split('|');
                    if (parts.Length == 2)
                    {
                        var oldFilePath = Path.Combine(folderPath, parts[0]);
                        var newFilePath = Path.Combine(folderPath, parts[1]);
                        if (File.Exists(newFilePath))
                        {
                            try
                            {
                                File.Move(newFilePath, oldFilePath);
                            }
                            catch (IOException ex)
                            {
                                // Handle IO exceptions, such as file access issues
                                Console.WriteLine($"IO Exception for {newFilePath} while undoing the file rename: {ex.Message}");
                                success = false;
                            }
                            catch (UnauthorizedAccessException ex)
                            {
                                // Handle unauthorized access exceptions
                                Console.WriteLine($"Unauthorized Access Exception for {newFilePath} while undoing the file rename: {ex.Message}");
                                success = false;
                            }
                            catch (Exception ex)
                            {
                                // Handle any other exceptions
                                Console.WriteLine($"Exception for {newFilePath} while undoing the file rename: {ex.Message}");
                                success = false;
                            }
                        }
                    }
                }

                if (success)
                {
                    File.Delete(randomTextFilePath);
                }
            }

            return success;
        }

        public static bool RandomlyRenameFiles(Dictionary<string, string> fileLookUp, List<string> originalFiles)
        {
            bool success = true;

            if (originalFiles.Count == 0)
            {
                return success;
            }

            Random random = new Random();

            string fullRandomPath = Path.Combine(Path.GetDirectoryName(originalFiles[0]), BackupRandomFileName);

            using (StreamWriter writer = new StreamWriter(fullRandomPath))
            {
                foreach (var file in originalFiles)
                {
                    try
                    {
                        var name = $"{originalFiles.IndexOf(file).ToString("00000")}_{random.Next(1000)}.mp3";
                        var finalName = Path.Combine(Path.GetDirectoryName(file), name);

                        // Write to file
                        writer.WriteLine($"{file}|{finalName}");
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