using System;
using System.Linq;
using System.Collections.Generic;
using FubuMVC.Core.Resources.Conneg;

namespace FubuMVC.Core.Registration.Conventions
{
    [Policy]
    public class DefaultOutputPolicy : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            graph.Behaviors
                .Where(x => !x.IsPartialOnly)
                .Where(x => x.HasResourceType() && !x.HasOutput())
                .Each(x => x.ApplyConneg());
        }
    }
}