# MP3GainMT TODO Features

This document outlines features from the original MP3Gain VBA implementation that are currently missing or incomplete in the C# `MP3GainMT` port.

## Core Gain Adjustment Modes
- [ ] **Track Gain vs Album Gain Selection:** The current UI applies Album Gain to the selected folder, missing the distinct `Apply Track Gain` (files adjusted independently) vs `Apply Album Gain` (files adjusted relative to the album peak) application modes available in the original tool.
- [ ] **Apply Constant Gain:** A feature to manually adjust the gain of files by an absolute, specific dB amount (in 1.5 dB increments), ignoring the Target Volume.
- [ ] **Single Channel Gain:** Support for applying gain modifications exclusively to the Left (`/l 0`) or Right (`/l 1`) channel of stereo files.
- [ ] **Max No-clip Gain Application:** The ability to execute `Apply Max No-clip Gain for Each file` and `Apply Max No-clip Gain for Album`, allowing users to peak-normalize files to their maximum safe volume before clipping.

## File and Folder Management
- [ ] **Add Individual Files:** The original tool has "Add File(s)" to select specific MP3s. Currently, the C# port only supports "Add Folder" via `folderPathTextBox`.
- [ ] **Add Subfolders Toggle:** A setting to optionally include or exclude recursive subfolders when adding a directory.
- [ ] **Clear Selected Files:** The C# UI has a `Clear All` button, but needs a way to remove specific selected rows without clearing the entire workspace.

## Backend CLI Flags and Settings
- [ ] **Ignore Clipping Warning (`/c`):** A UI toggle to forcefully apply gain even when it would cause clipping.
- [ ] **Preserve Timestamps (`/p`):** A UI toggle to retain the original file's `Date Modified` attribute after writing changes.
- [ ] **Use Temp Files (`/t`):** A UI toggle enabling writes to a temporary file before overwriting the original, mitigating risk if the process crashes mid-write.

## Tag Management
- [ ] **Remove Tags from files (`/s d`):** A dedicated action to delete MP3Gain APE/ID3 tags.
- [ ] **Ignore/Skip Tags (`/s s`):** A toggle to completely bypass reading or writing APE/ID3 tags during analysis.
- [ ] **Re-calculate/Force Analysis (`/s r`):** A toggle to force MP3Gain to re-analyze files instead of relying on existing tags.
- [ ] **Don't check while adding files:** A setting to speed up massive folder loading by skipping the tag check step until analysis is explicitly requested.