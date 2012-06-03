using System;
using System.Linq;
using System.Collections.Generic;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Registration.Conventions
{
    [Policy]
    public class PartialOnlyConvention : IConfigurationAction
    {
        public const string Partial = "Partial";


        public void Configure(BehaviorGraph graph)
        {
            graph
                .Actions()
                .Where(ShouldBePartial)
                .Select(x => x.ParentChain())
                .Each(x => x.IsPartialOnly = true);
        }

        public static bool ShouldBePartial(ActionCall call)
        {
            return call.Method.Name.EndsWith(Partial);
        }
    }
}