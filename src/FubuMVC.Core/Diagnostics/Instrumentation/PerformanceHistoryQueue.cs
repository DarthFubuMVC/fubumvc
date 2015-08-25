using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FubuMVC.Core.Diagnostics.Packaging;
using StructureMap.Configuration.DSL;

namespace FubuMVC.Core.Diagnostics.Instrumentation
{
    public class InMemoryExecutionLogStorage : IExecutionLogStorage
    {
        private readonly IChainExecutionHistory _history;
        private readonly PerformanceHistoryQueue _queue;

        public InMemoryExecutionLogStorage(IChainExecutionHistory history, PerformanceHistoryQueue queue)
        {
            _history = history;
            _queue = queue;
        }

        public void Store(IChainExecutionLog log)
        {
            _history.Store((ChainExecutionLog) log);
            _queue.Enqueue((ChainExecutionLog) log);
        }
    }

    public interface IChainExecutionHistory
    {
        void Store(ChainExecutionLog log);
        IEnumerable<ChainExecutionLog> RecentReports();
        string CurrentSessionTag { get; set; }
        ChainExecutionLog Find(Guid id);
        void Clear();
    }

    public class ChainExecutionHistory : IChainExecutionHistory
    {
        private readonly ConcurrentQueue<ChainExecutionLog> _reports = new ConcurrentQueue<ChainExecutionLog>();
        private readonly DiagnosticsSettings _settings;

        public ChainExecutionHistory(DiagnosticsSettings settings)
        {
            _settings = settings;
        }

        public void Store(ChainExecutionLog log)
        {
            log.SessionTag = CurrentSessionTag;

            _reports.Enqueue(log);
            while (_reports.Count > _settings.MaxRequests)
            {
                ChainExecutionLog _;
                _reports.TryDequeue(out _);
            }
        }

        public IEnumerable<ChainExecutionLog> RecentReports()
        {
            return _reports.ToList();
        }

        public string CurrentSessionTag { get; set; }

        public ChainExecutionLog Find(Guid id)
        {
            return _reports.FirstOrDefault(x => x.Id == id);
        }

        public void Clear()
        {
            while (_reports.Count > 0)
            {
                ChainExecutionLog _;
                _reports.TryDequeue(out _);
            }
        }
    }

    public class InMemoryInstrumentationServices : Registry
    {
        public InMemoryInstrumentationServices()
        {
            ForSingletonOf<IExecutionLogStorage>().Use<InMemoryExecutionLogStorage>();

            For<IActivator>().Add<PerformanceHistoryQueueActivator>();

            ForSingletonOf<IPerformanceHistoryQueue>().Use<PerformanceHistoryQueue>();
            ForSingletonOf<IChainExecutionHistory>().Use<ChainExecutionHistory>();
        }
    }


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

    // Break it out. This needs to be a singleton
    public interface IPerformanceHistoryQueue
    {
        void Dispose();
        void Enqueue(ChainExecutionLog log);
        void Start();
    }

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
            foreach (var request in _collection.GetConsumingEnumerable())
            {
                // record the request in the new in memory log
                throw new Exception("Do something here.");
            }
        }

        public void Start()
        {
            _readingTask = Task.Factory.StartNew(record);
        }
    }
}