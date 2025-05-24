using MP3GainMT.Interfaces;
using System;
using System.Diagnostics;

namespace MP3GainMT.Wrappers
{
    public class ProcessWrapper : IProcess
    {
        private readonly Process _process;

        public ProcessWrapper()
        {
            _process = new Process();
        }

        public ProcessStartInfo StartInfo
        {
            get => _process.StartInfo;
            set => _process.StartInfo = value;
        }

        public bool EnableRaisingEvents
        {
            get => _process.EnableRaisingEvents;
            set => _process.EnableRaisingEvents = value;
        }

        public int ExitCode => _process.ExitCode;

        public event DataReceivedEventHandler OutputDataReceived
        {
            add => _process.OutputDataReceived += value;
            remove => _process.OutputDataReceived -= value;
        }

        public event DataReceivedEventHandler ErrorDataReceived
        {
            add => _process.ErrorDataReceived += value;
            remove => _process.ErrorDataReceived -= value;
        }

        public void BeginErrorReadLine() => _process.BeginErrorReadLine();
        public void BeginOutputReadLine() => _process.BeginOutputReadLine();
        public bool Start() => _process.Start();
        public bool WaitForExit(int milliseconds) => _process.WaitForExit(milliseconds);
        public void WaitForExit() => _process.WaitForExit();

        public void Close() => _process.Close(); // Calls Dispose internally for Process

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _process?.Dispose();
            }
        }
    }
}
