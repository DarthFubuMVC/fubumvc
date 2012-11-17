using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Registration.Conventions
{
    [Policy]
    [Description("Applies modifications to an ActionCall from any attribute on an action call that inherits from ModifyChainAttribute")]
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