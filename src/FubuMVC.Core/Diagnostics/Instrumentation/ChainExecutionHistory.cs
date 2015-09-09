using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Diagnostics.Instrumentation
{
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

        public IEnumerable<ChainExecutionLog> GetRecentRequests(BehaviorChain chain)
        {
            return _reports.ToArray().Where(x => x.RootChain == chain).ToArray();
        }
    }
}