using System;

namespace FubuMVC.Core.ServerSentEvents
{
    public class NulloShutdownDetector : IAspNetShutDownDetector
    {
        public void Stop(bool immediate)
        {
        }

        public void Dispose()
        {
        }

        public void Register(Action onShutdown)
        {
        }
    }
}