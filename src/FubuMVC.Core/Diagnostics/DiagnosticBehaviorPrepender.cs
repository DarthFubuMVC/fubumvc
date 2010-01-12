using System;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using System.Linq;

namespace FubuMVC.Core.Diagnostics
{
    public class DiagnosticBehaviorPrepender : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            graph.Behaviors.Each(modifyChain);
        }

        private static void modifyChain(BehaviorChain chain)
        {
            chain.ToArray().Each(node => node.InsertDirectlyBefore(Wrapper.For<BehaviorTracer>()));
            chain.Prepend(new Wrapper(typeof (DiagnosticBehavior)));
        }
    }
}