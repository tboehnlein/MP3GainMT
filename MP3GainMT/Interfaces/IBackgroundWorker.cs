using System;
using System.ComponentModel;

namespace MP3GainMT.Interfaces
{
    public interface IBackgroundWorker : IDisposable
    {
        bool IsBusy { get; }
        bool WorkerReportsProgress { get; set; }
        bool WorkerSupportsCancellation { get; set; }

        event DoWorkEventHandler DoWork;
        event ProgressChangedEventHandler ProgressChanged;
        event RunWorkerCompletedEventHandler RunWorkerCompleted;

        void CancelAsync();
        void RunWorkerAsync();
        void RunWorkerAsync(object argument);
        void ReportProgress(int percentProgress);
        void ReportProgress(int percentProgress, object userState);
    }
}
