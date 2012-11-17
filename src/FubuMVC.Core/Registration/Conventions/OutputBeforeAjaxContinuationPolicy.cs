using FubuCore.Descriptions;
using FubuMVC.Core.Ajax;
using System.Linq;
using System.Collections.Generic;
using FubuMVC.Core.Registration.Nodes;
using FubuCore;
using FubuMVC.Core.Resources.Conneg;

namespace FubuMVC.Core.Registration.Conventions
{
    [Title("Reorder the chain for special semantics when the resource type is AjaxContinuation")]
    public class OutputBeforeAjaxContinuationPolicy : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            graph.Behaviors
                .Where(x => x.ResourceType().CanBeCastTo<AjaxContinuation>())
                .ToList()
                .Each(Modify);

            
        }

        public static void Modify(BehaviorChain chain)
        {
            // Not everything is hard
            chain.Output.Remove();

            if (chain.OfType<InputNode>().Any())
            {
                chain.Input.AddAfter(chain.Output);
            }
            else
            {
                chain.FirstCall().AddBefore(chain.Output);
            }
        }
    }
}