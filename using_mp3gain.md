# MP3Gain / AACGain Command Line Usage

```text
aacgain version 1.9.0, derived from mp3gain version 1.5.2
copyright(c) 2001-2009 by Glen Sawyer
AAC support copyright(c) 2004-2009 David Lasker, Altos Design, Inc.
uses mpglib, which can be found at http://www.mpg123.de
AAC support uses faad2 (http://www.audiocoding.com), and
mpeg4ip's mp4v2 (http://www.mpeg4ip.net)
Usage: C:\Program Files (x86)\MP3Gain\mp3gain.exe [options] <infile> [<infile 2> ...]
options:
        /v - show version number
        /g <i>  - apply gain i without doing any analysis
        /l 0 <i> - apply gain i to channel 0 (left channel)
                  without doing any analysis (ONLY works for STEREO files,
                  not Joint Stereo)
        /l 1 <i> - apply gain i to channel 1 (right channel)
        /e - skip Album analysis, even if multiple files listed
        /r - apply Track gain automatically (all files set to equal loudness)
        /k - automatically lower Track/Album gain to not clip audio
        /a - apply Album gain automatically (files are all from the same
                      album: a single gain change is applied to all files, so
                      their loudness relative to each other remains unchanged,
                      but the average album loudness is normalized)
        /m <i> - modify suggested MP3 gain by integer i
        /d <n> - modify suggested dB gain by floating-point n
        /c - ignore clipping warning when applying gain
        /o - output is a database-friendly tab-delimited list
        /t - writes modified data to temp file, then deletes original
             instead of modifying bytes in original file
             A temp file is always used for AAC files.
        /q - Quiet mode: no status messages
        /p - Preserve original file timestamp
        /x - Only find max. amplitude of file
        /f - Assume input file is an MPEG 2 Layer III file
             (i.e. don't check for mis-named Layer I or Layer II files)
              This option is ignored for AAC files.
        /? or /h - show this message
        /s c - only check stored tag info (no other processing)
        /s d - delete stored tag info (no other processing)
        /s s - skip (ignore) stored tag info (do not read or write tags)
        /s r - force re-calculation (do not read tag info)
        /s i - use ID3v2 tag for MP3 gain info
        /s a - use APE tag for MP3 gain info (default)
        /u - undo changes made (based on stored tag info)
        /w - "wrap" gain change if gain+change > 255 or gain+change < 0
              MP3 only. (use "/? wrap" switch for a complete explanation)
If you specify /r and /a, only the second one will work
If you do not specify /c, the program will stop and ask before
     applying gain change to a file that might clip
```

## MP3Gain GUI Features Summary

MP3Gain analyzes MP3 files to calculate how loud they sound to the human ear (using the Replay Gain algorithm) and adjusts them losslessly so they all have the same loudness. It does this by modifying the "global gain" field in each MP3 frame. Since the global gain field is an 8-bit integer, adjustments are made in completely lossless **1.5 dB steps**.

### Core Modes & Analysis
*   **Track Mode / Track Analysis:** Volume-corrects a mix of unrelated songs to a selected Target Volume. Each song is adjusted individually so its average volume matches the target.
    *   **CLI Equivalent:** Output retrieved via `/o` flag (tab-delimited output). The GUI parses this to show track volume.
*   **Album Mode / Album Analysis:** Volume-corrects a collection of related songs (an album) relative to other albums. The overall volume of the album is adjusted to the Target Volume, but the volume differences between the individual tracks are preserved.
    *   **CLI Equivalent:** Output retrieved via `/o` flag. The GUI uses this to calculate Album Volume.
*   **Max No-clip Analysis:** Finds the peak amplitude of each MP3 without performing the full Replay Gain loudness calculation. Used solely to identify how much gain can be applied without clipping.
    *   **CLI Equivalent:** Corresponds to the `/x` flag (Only find max. amplitude of file).

### Target Volume & Adjustments
The default Target Volume is **89.0 dB** because most MP3s will not clip at this level. Setting this higher runs the risk of inducing clipping. The GUI offsets the default MP3Gain calculations to account for a user's customized Target Volume before using the backend CLI to apply specific step modifications.

### Modify Gain Features
*   **Apply Track Gain:** Adjusts the gain of each file individually to match the Target Volume.
    *   **CLI Equivalent:** The GUI calculates the exact 1.5 dB steps required and applies it with `/g <i>` (or uses `/r`).
*   **Apply Album Gain:** Adjusts the gain of the entire album by the same amount so the average album volume matches the Target Volume.
    *   **CLI Equivalent:** The GUI calculates the album step adjustment and applies it with `/g <i>` to all files (or uses `/a`).
*   **Apply Constant Gain:** Adjusts the gain of files by an absolute, specific dB amount (in 1.5 dB increments), regardless of the Target Volume. Also supports applying gain to a single channel (Left/Right) for stereo files.
    *   **CLI Equivalent:** Uses `/g <i>` for all channels, or `/l 0 <i>` (Left) / `/l 1 <i>` (Right) for single-channel adjustments.
*   **Apply Max No-clip Gain for Each file:** Adjusts each file so it is as loud as possible without clipping (peak normalization). 
    *   **CLI Equivalent:** Analyzes via `/x` and applies the max safe gain with `/g <i>`.
*   **Apply Max No-clip Gain for Album:** Finds the highest gain that can be applied to an entire folder without *any* track clipping, and applies that single gain change to all tracks.
    *   **CLI Equivalent:** Derived from analyzing all files, identifying the lowest maximum-safe gain, and applying it via `/g <i>`.
*   **Undo Gain changes:** Reverts the gain changes losslessly using the stored tag information.
    *   **CLI Equivalent:** Corresponds to the `/u` flag.

### Tag Management
MP3Gain stores analysis and undo information in tags within the MP3 file itself (APE tags by default). This prevents the need to re-analyze files in the future.
*   **Ignore (do not read or write tags):** Completely turns off all tag features.
    *   **CLI Equivalent:** Corresponds to the `/s s` flag.
*   **Re-calculate (do not read tags):** Force MP3Gain to re-analyze even if tags exist.
    *   **CLI Equivalent:** Corresponds to the `/s r` flag.
*   **Don't check while adding files:** Speeds up loading large folders by not checking for tags until analysis/gain is requested.
    *   **CLI Equivalent:** *This is a GUI-level optimization.* The GUI defers calling `/s c` (check stored tag info) until the user requests action.
*   **Remove Tags from files:** Deletes MP3Gain tags (useful for compatibility with certain players).
    *   **CLI Equivalent:** Corresponds to the `/s d` flag.

### Global Options / Other Backend Flags
*   **Ignore clipping warning when applying gain:** 
    *   **CLI Equivalent:** Uses the `/c` flag to allow the backend to apply changes even if it induces clipping.
*   **Quiet mode:** 
    *   **CLI Equivalent:** Uses the `/q` flag to suppress status messages (used heavily by the GUI to parse clean data).
*   **Preserve original file timestamp:** 
    *   **CLI Equivalent:** Uses the `/p` flag to keep the original modified date on the file.
*   **Output is a database-friendly tab-delimited list:**
    *   **CLI Equivalent:** Uses the `/o` flag. The GUI heavily relies on this to format strings returned by `mp3gain.exe` and display them in the visual grid.
*   **Write modified data to temp file:**
    *   **CLI Equivalent:** Uses the `/t` flag. Ensures the original file is kept safe while writing data, swapping the temp file at the end.

### File List Interface
The interface lists files and color-codes rows in **RED** if a file is currently clipping, or if applying the suggested Track/Album gain will cause it to clip. A `???` in the clipping column indicates bad data/extreme clipping in the file.
*   **Volume / Album Volume:** Current perceived loudness in dB SPL.
*   **Track Gain / Album Gain:** The suggested change in 1.5 dB steps required to reach the target volume.
*   **Clipping / clip(Track) / clip(Album):** Indicates if the file clips now or will clip after the specified gain is applied.
*   **Max Noclip Gain:** The maximum gain that can be applied without causing clipping.