using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Diagnostics
{
    [ConfigurationType(ConfigurationType.Instrumentation)]
    public class ApplyTracing : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            foreach (BehaviorChain chain in graph.Behaviors)
            {
                ApplyToChain(chain);
            }
        }

        public static void ApplyToChain(BehaviorChain chain)
        {
            var nodes = chain.ToList();
            nodes.Each(x => new BehaviorTracerNode(x));

            if (!chain.IsPartialOnly)
            {
                new DiagnosticNode(chain);
            }
        }
    }
}