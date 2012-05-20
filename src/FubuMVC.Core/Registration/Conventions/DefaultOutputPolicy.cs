using System;
using System.Linq;
using System.Collections.Generic;
using FubuMVC.Core.Resources.Conneg;

namespace FubuMVC.Core.Registration.Conventions
{
    public class DefaultOutputPolicy : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            graph.Behaviors
                .Where(x => x.HasResourceType() && !x.HasOutput())
                .Each(x => x.ApplyConneg());
        }
    }
}