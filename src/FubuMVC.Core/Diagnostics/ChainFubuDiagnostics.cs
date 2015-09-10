using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuCore.Descriptions;
using FubuMVC.Core.Diagnostics.Endpoints;
using FubuMVC.Core.Diagnostics.Instrumentation;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Diagnostics
{
    public class ChainFubuDiagnostics
    {
        private readonly BehaviorGraph _graph;
        private readonly IChainExecutionHistory _history;

        public ChainFubuDiagnostics(BehaviorGraph graph, IChainExecutionHistory history)
        {
            _graph = graph;
            _history = history;
        }

        public Dictionary<string, object> get_chain_details_Hash(ChainDetailsRequest request)
        {
            var dict = new Dictionary<string, object>();

            var chain = _graph.FindChain(request.Hash);
            if (chain == null)
            {
                dict.Add("not-found", true);
                return dict;
            }

            dict.Add("details", EndpointReport.ForChain(chain).ToDictionary());

            if (chain is RoutedChain)
            {
                var routed = chain.As<RoutedChain>();
                var description = Description.For(routed.Route);


                dict.Add("route", description.ToDictionary());
            }

            var nodes = chain.NonDiagnosticNodes().Select(x =>
            {
                var details = x.ToDescriptiveDictionary();
                details.Add("category", x.Category.ToString());


                return details;
            });

            addPerformanceData(dict, chain);

            dict.Add("nodes", nodes);


            return dict;
        }

        private void addPerformanceData(Dictionary<string, object> dict, BehaviorChain chain)
        {
            dict.Add("performance", chain.Performance.ToDictionary());

            var recent = _history.GetRecentRequests(chain).OrderByDescending(x => x.Time);
            var executions = recent.Select(log =>
            {
                var logDict = log.ToHeaderDictionary();
                logDict.Add("warn", chain.Performance.IsWarning(log));

                return logDict;
            }).ToArray();

            dict.Add("executions", executions);
        }
    }

    public class ChainDetailsRequest
    {
        public int Hash { get; set; }

        public bool Equals(ChainDetailsRequest other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.Hash.Equals(Hash);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(ChainDetailsRequest)) return false;
            return Equals((ChainDetailsRequest)obj);
        }

        public override int GetHashCode()
        {
            return Hash.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("Hash: {0}", Hash);
        }
    }
}