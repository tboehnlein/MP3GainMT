using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MP3GainMT.Helpers
{
    public class FileRenamer : IFileRenamer
    {
        // Moved from Helpers.cs, made private as it's an implementation detail.
        private const string BackupRandomFileName = "random_file.txt";

        public bool RandomlyRenameFiles(Dictionary<string, string> fileLookUp, List<string> originalFiles)
        {
            bool success = true;

            if (originalFiles == null || originalFiles.Count == 0)
            {
                return success;
            }
            if (fileLookUp == null)
            {
                throw new ArgumentNullException(nameof(fileLookUp));
            }
            fileLookUp.Clear(); // Ensure it's empty before populating

            Random random = new Random();

            // Ensure there's a directory to write the backup file
            string directoryPath = Path.GetDirectoryName(originalFiles[0]);
            if (string.IsNullOrEmpty(directoryPath) || !Directory.Exists(directoryPath))
            {
                // Or handle error appropriately: throw, log, etc.
                Console.WriteLine($"Error: Directory for original files does not exist or is invalid: {directoryPath}");
                return false; 
            }
            string fullRandomPath = Path.Combine(directoryPath, BackupRandomFileName);


            using (StreamWriter writer = new StreamWriter(fullRandomPath))
            {
                for (int i = 0; i < originalFiles.Count; i++)
                {
                    string file = originalFiles[i];
                    try
                    {
                        // Use index for a more unique base for the random name
                        var name = $"{i:D5}_{random.Next(100000, 999999)}.mp3";
                        var finalName = Path.Combine(Path.GetDirectoryName(file), name);

                        writer.WriteLine($"{Path.GetFileName(file)}|{name}"); // Store original name and new random name
                        writer.Flush();
                        
                        File.Move(file, finalName);
                        fileLookUp.Add(finalName, file); // Maps new random name back to original full path
                    }
                    catch (IOException ex)
                    {
                        Console.WriteLine($"IO Exception for {file} while applying the file rename: {ex.Message}");
                        success = false;
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        Console.WriteLine($"Unauthorized Access Exception for {file} while applying the file rename: {ex.Message}");
                        success = false;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Exception for {file} while applying the file rename: {ex.Message}");
                        success = false;
                    }

                    if (!success)
                    {
                        // Attempt to undo what's been done so far on failure
                        UndoRandomlyRenameFiles(fileLookUp); // Pass the current state of fileLookUp
                        break;
                    }
                }
            }

            return success;
        }

        public void UndoRandomlyRenameFiles(Dictionary<string, string> fileLookUp)
        {
            if (fileLookUp == null || fileLookUp.Count == 0)
            {
                return;
            }

            string directoryPath = Path.GetDirectoryName(fileLookUp.Values.First());
            if (string.IsNullOrEmpty(directoryPath)) return; // Should not happen if fileLookUp is populated correctly

            string randomFilePath = Path.Combine(directoryPath, BackupRandomFileName);

            // In the new scheme, fileLookUp maps: newName -> originalFullPath
            // We need to iterate it and move newName back to originalFullPath's Name in the same directory.
            foreach (var entry in fileLookUp)
            {
                string newNamePath = entry.Key; // Current path (randomly named)
                string originalFullPath = entry.Value; // Original full path

                try
                {
                    if (File.Exists(newNamePath))
                    {
                        File.Move(newNamePath, originalFullPath);
                    }
                }
                catch (IOException ex)
                {
                    Console.WriteLine($"IO Exception for {newNamePath} while undoing the file rename to {originalFullPath}: {ex.Message}");
                }
                catch (UnauthorizedAccessException ex)
                {
                    Console.WriteLine($"Unauthorized Access Exception for {newNamePath} while undoing the file rename to {originalFullPath}: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception for {newNamePath} while undoing the file rename to {originalFullPath}: {ex.Message}");
                }
            }

            try
            {
                if (File.Exists(randomFilePath))
                {
                    File.Delete(randomFilePath);
                }
            }
            catch (IOException ex)
            {
                 Console.WriteLine($"IO Exception while deleting backup file {randomFilePath}: {ex.Message}");
            }
             catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Unauthorized Access Exception while deleting backup file {randomFilePath}: {ex.Message}");
            }
        }
    }
}
