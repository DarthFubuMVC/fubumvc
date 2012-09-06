using System;
using System.Linq;
using System.Collections.Generic;
using FubuCore;
using FubuMVC.Core.Continuations;

namespace FubuMVC.Core.Registration.Conventions
{
    [Policy]
    public class AttachOutputPolicy : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            graph.Behaviors.Where(x => x.ResourceType() != null && x.ResourceType() != typeof(void) && 
                !x.ResourceType().CanBeCastTo<FubuContinuation>() && x.Output.Writers.Any()).Each(x => x.AddToEnd(x.Output));
        }
    }
}