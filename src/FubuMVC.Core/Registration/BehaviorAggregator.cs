using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Registration
{
    public class BehaviorAggregator : IConfigurationAction
    {
        private readonly TypePool _types;
        private readonly IEnumerable<IActionSource> _sources;

        public BehaviorAggregator(TypePool types, IEnumerable<IActionSource> sources)
        {
            _types = types;
            _sources = sources;
        }

        public void Configure(BehaviorGraph graph)
        {
            _sources
                .SelectMany(src => src.FindActions(_types))
                .Each(call =>
                          {
                              var chain = new BehaviorChain();
                              chain.AddToEnd(call);
                              graph.AddChain(chain);
                          });
        }
    }
}