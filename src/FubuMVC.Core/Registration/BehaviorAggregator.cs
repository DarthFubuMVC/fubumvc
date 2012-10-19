using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Registration
{
    [ConfigurationType(ConfigurationType.Discovery)]
    public class BehaviorAggregator : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            var sources = graph.Settings.Get<ActionSources>().AllSources();

            sources
                .SelectMany(src => src.FindActions(graph.Types))
                .Distinct()
                .Each(call => {
                    var chain = new BehaviorChain();
                    chain.AddToEnd(call);
                    graph.AddChain(chain);
                });
        }
    }
}