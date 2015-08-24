using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using FubuMVC.Core.Diagnostics.Packaging;

namespace FubuMVC.Core.Diagnostics.Instrumentation
{
    public interface IExecutionRecorder
    {
        void Record(ChainExecutionLog log);
    }


    // This will need to be registered. Will make it a singleton
    // just to keep from making it over and over again
    public class InMemoryExecutionRecorder : IExecutionRecorder
    {
        private readonly PerformanceHistoryQueue _queue;

        public InMemoryExecutionRecorder(PerformanceHistoryQueue queue)
        {
            _queue = queue;
        }

        public void Record(ChainExecutionLog log)
        {
            _queue.Enqueue(log);
        }
    }

    // TODO -- this will need to be registered
    public class PerformanceHistoryRecording : IActivator
    {
        private readonly PerformanceHistoryQueue _queue;

        public PerformanceHistoryRecording(PerformanceHistoryQueue queue)
        {
            _queue = queue;
        }

        public void Activate(IActivationLog log, IPerfTimer timer)
        {
            log.Trace("Starting the in memory performance tracking");
            _queue.Start();
        }
    }

    public class PerformanceHistoryQueue : IDisposable
    {
        private readonly BlockingCollection<ChainExecutionLog> _collection =
            new BlockingCollection<ChainExecutionLog>(new ConcurrentBag<ChainExecutionLog>());

        private Task _readingTask;

        public void Dispose()
        {
            _collection.CompleteAdding();
            _collection.Dispose();
        }


        public void Enqueue(ChainExecutionLog log)
        {
            _collection.Add(log);
        }

        private void record()
        {
            foreach (var request in _collection.GetConsumingEnumerable())
            {
                // TODO -- apply the history
            }
        }

        public void Start()
        {
            _readingTask = Task.Factory.StartNew(record);
        }
    }
}