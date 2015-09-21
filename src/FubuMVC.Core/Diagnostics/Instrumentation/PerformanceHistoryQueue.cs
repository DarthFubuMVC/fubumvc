using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace FubuMVC.Core.Diagnostics.Instrumentation
{
    // Break it out. This needs to be a singleton

    public class PerformanceHistoryQueue : IDisposable, IPerformanceHistoryQueue
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
            foreach (var log in _collection.GetConsumingEnumerable())
            {
                try
                {
                    if (log.RootChain != null)
                    {
                        log.RootChain.Performance.Read(log);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        public void Start()
        {
            _readingTask = Task.Factory.StartNew(record);
        }
    }
}