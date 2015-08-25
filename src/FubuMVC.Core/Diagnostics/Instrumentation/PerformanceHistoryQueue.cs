using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace FubuMVC.Core.Diagnostics.Instrumentation
{
    public class PerformanceHistoryQueue : IExecutionLogStorage, IDisposable
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
                throw new Exception("Do something here.");
            }
        }

        public void Start()
        {
            _readingTask = Task.Factory.StartNew(record);
        }

        void IExecutionLogStorage.Store(IChainExecutionLog log)
        {
            Enqueue((ChainExecutionLog) log);
        }
    }
}