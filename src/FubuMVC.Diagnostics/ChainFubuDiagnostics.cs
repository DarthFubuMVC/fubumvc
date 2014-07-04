using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Diagnostics.Endpoints;
using FubuMVC.Diagnostics.Visualization;

namespace FubuMVC.Diagnostics
{
    public class ChainFubuDiagnostics
    {
        private readonly IVisualizer _visualizer;
        private readonly BehaviorGraph _graph;

        public ChainFubuDiagnostics(BehaviorGraph graph, IVisualizer visualizer)
        {
            _graph = graph;
            _visualizer = visualizer;
        }

        public Dictionary<string, object> get_chain_details_Hash(ChainDetailsRequest request)
        {
            var dict = new Dictionary<string, object>();

            var chain = _graph.Behaviors.FirstOrDefault(x => x.GetHashCode() == request.Hash);
            if (chain == null)
            {
                dict.Add("not-found", true);
                return dict;
            }

            dict.Add("details", EndpointReport.ForChain(chain).ToDictionary());

            if (chain is RoutedChain)
            {
                var routed = chain.As<RoutedChain>();
                dict.Add("route", routed.Route.ToDictionary());
            }

            var nodes = chain.NonDiagnosticNodes().Select(x => _visualizer.Visualize(x).ToString()).ToArray();

            dict.Add("nodes", nodes);


            return dict;
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