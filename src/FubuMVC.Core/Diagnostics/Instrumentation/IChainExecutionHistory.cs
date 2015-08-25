using System;
using System.Collections.Generic;

namespace FubuMVC.Core.Diagnostics.Instrumentation
{
    public interface IChainExecutionHistory
    {
        void Store(ChainExecutionLog log);
        IEnumerable<ChainExecutionLog> RecentReports();
        string CurrentSessionTag { get; set; }
        ChainExecutionLog Find(Guid id);
        void Clear();
    }
}