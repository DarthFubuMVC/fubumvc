using System.Collections.Generic;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Registration.Conventions
{
    public class ModifyChainAttributeConvention : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            graph.Behaviors.Each(analyzeChain);
        }

        private static void analyzeChain(BehaviorChain chain)
        {
            chain.Calls.Each(call => call.ForAttributes<ModifyChainAttribute>(att => att.Alter(call)));
        }
    }
}