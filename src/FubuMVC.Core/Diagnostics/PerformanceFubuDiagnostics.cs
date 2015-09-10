using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Diagnostics.Instrumentation;
using FubuMVC.Core.Registration;

namespace FubuMVC.Core.Diagnostics
{
    public class PerformanceFubuDiagnostics
    {
        private readonly BehaviorGraph _graph;
        private readonly IChainExecutionHistory _history;

        public PerformanceFubuDiagnostics(BehaviorGraph graph, IChainExecutionHistory history)
        {
            _graph = graph;
            _history = history;
        }

        public Dictionary<string, object>[] get_performance()
        {
            return _graph.Chains
                .Where(x => x.Performance.HitCount > 0)
                .OrderByDescending(x => x.Performance.TotalExecutionTime)
                .Select(chain =>
            {
                var dict = chain.Performance.ToDictionary();
                dict.Add("title", chain.Title());
                dict.Add("hash", chain.Key);

                return dict;
            }).ToArray();
        }


    }

    public class PerformanceByChain
    {
        public int Hash { get; set; }
    }
}