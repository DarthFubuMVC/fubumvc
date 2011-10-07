using System.Collections.Generic;
using System.Linq;
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
            chain.Calls.ToList().Each(call => call.ForAttributes<ModifyChainAttribute>(att => att.Alter(call)));
        }
    }
}