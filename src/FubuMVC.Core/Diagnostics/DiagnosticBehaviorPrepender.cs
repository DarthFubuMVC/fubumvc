using System.Collections.Generic;
using FubuCore;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using System.Linq;

namespace FubuMVC.Core.Diagnostics
{
    [WannaKill]
    public class DiagnosticBehaviorPrepender : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            graph.Behaviors.Each(c => modifyChain(c, graph.Observer));
        }

        private static void modifyChain(BehaviorChain chain, IConfigurationObserver observer)
        {
            chain.Calls.Each(c => observer.RecordCallStatus(c, "Wrapping with diagnostic tracer and behavior"));
            chain.ToArray().Each(node => node.AddBefore(Wrapper.For<BehaviorTracer>()));
            chain.Prepend(new Wrapper(typeof (DiagnosticBehavior)));
        }
    }
}