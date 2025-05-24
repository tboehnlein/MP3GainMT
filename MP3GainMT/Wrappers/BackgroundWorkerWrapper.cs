using MP3GainMT.Interfaces;
using System;
using System.ComponentModel;

namespace MP3GainMT.Wrappers
{
    public class BackgroundWorkerWrapper : IBackgroundWorker
    {
        private readonly BackgroundWorker _worker;

        public BackgroundWorkerWrapper()
        {
            _worker = new BackgroundWorker();
            // Forward events from the real BackgroundWorker to the interface's events
            _worker.DoWork += (sender, e) => DoWork?.Invoke(this, e);
            _worker.ProgressChanged += (sender, e) => ProgressChanged?.Invoke(this, e);
            _worker.RunWorkerCompleted += (sender, e) => RunWorkerCompleted?.Invoke(this, e);
        }

        public bool IsBusy => _worker.IsBusy;

        public bool WorkerReportsProgress
        {
            get => _worker.WorkerReportsProgress;
            set => _worker.WorkerReportsProgress = value;
        }

        public bool WorkerSupportsCancellation
        {
            get => _worker.WorkerSupportsCancellation;
            set => _worker.WorkerSupportsCancellation = value;
        }

        public event DoWorkEventHandler DoWork;
        public event ProgressChangedEventHandler ProgressChanged;
        public event RunWorkerCompletedEventHandler RunWorkerCompleted;

        public void CancelAsync() => _worker.CancelAsync();
        public void RunWorkerAsync() => _worker.RunWorkerAsync();
        public void RunWorkerAsync(object argument) => _worker.RunWorkerAsync(argument);
        public void ReportProgress(int percentProgress) => _worker.ReportProgress(percentProgress);
        public void ReportProgress(int percentProgress, object userState) => _worker.ReportProgress(percentProgress, userState);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _worker?.Dispose();
            }
        }
    }
}
