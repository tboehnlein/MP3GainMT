using MP3GainMT.Interfaces;
using MP3GainMT.Wrappers;

namespace MP3GainMT.Factories
{
    public class BackgroundWorkerFactory : IBackgroundWorkerFactory
    {
        public IBackgroundWorker Create()
        {
            return new BackgroundWorkerWrapper();
        }
    }
}
