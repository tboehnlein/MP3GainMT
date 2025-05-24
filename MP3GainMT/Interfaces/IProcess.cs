using System;
using System.Diagnostics;

namespace MP3GainMT.Interfaces
{
    public interface IProcess : IDisposable
    {
        ProcessStartInfo StartInfo { get; set; }
        bool EnableRaisingEvents { get; set; }
        int ExitCode { get; } // Added for completeness, though not explicitly in list, often needed.
        // Consider if StandardOutput/StandardError streams are needed directly, or if events are sufficient.
        // For now, focusing on event-based reading.

        event DataReceivedEventHandler OutputDataReceived;
        event DataReceivedEventHandler ErrorDataReceived;

        bool Start();
        void BeginOutputReadLine();
        void BeginErrorReadLine();
        bool WaitForExit(int milliseconds); // Making timeout explicit, original uses WaitForExit()
        void WaitForExit(); // Overload for indefinite wait
        void Close(); // From IDisposable, but also Process has Close()
        // Potentially Kill() if needed for error handling
    }
}
