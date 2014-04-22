using System;
using FubuCore.Descriptions;
using FubuMVC.Core.Ajax;
using System.Linq;
using System.Collections.Generic;
using FubuMVC.Core.Registration.Nodes;
using FubuCore;
using FubuMVC.Core.Resources.Conneg;

namespace FubuMVC.Core.Registration.Conventions
{
    [Obsolete("Make this unnecessary")]
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

        // TODO -- got to be a smarter way to do this.
        public static void Modify(BehaviorChain chain)
        {
            // Not everything is hard
            var output = chain.Output.As<BehaviorNode>();
            output.Remove();

            if (chain.OfType<InputNode>().Any())
            {
                chain.Input.As<BehaviorNode>().AddAfter(output);
            }
            else
            {
                chain.FirstCall().AddBefore(output);
            }
        }
    }
}