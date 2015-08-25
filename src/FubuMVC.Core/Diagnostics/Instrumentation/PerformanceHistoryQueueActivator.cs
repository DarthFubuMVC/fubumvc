using FubuMVC.Core.Diagnostics.Packaging;

namespace FubuMVC.Core.Diagnostics.Instrumentation
{
    public class PerformanceHistoryQueueActivator : IActivator
    {
        private readonly IPerformanceHistoryQueue _queue;

        public PerformanceHistoryQueueActivator(IPerformanceHistoryQueue queue)
        {
            _queue = queue;
        }

        public void Activate(IActivationLog log, IPerfTimer timer)
        {
            log.Trace("Starting the in memory performance history tracking");
            _queue.Start();
        }
    }
}