using FubuMVC.Core.Caching;
using FubuMVC.Core.Registration.Nodes;
using System.Linq;
using System.Collections.Generic;

namespace FubuMVC.Core.Registration.Conventions
{
    public class CachedPartialConvention : IConfigurationAction
    {
        public const string CachedPartial = "CachedPartial";

        public void Configure(BehaviorGraph graph)
        {
            graph
                .Actions()
                .Where(ShouldBeCachedPartial)
                .Select(x => x.ParentChain())
                .Each(Modify);

        }

        public static void Modify(BehaviorChain chain)
        {
            if (chain.OfType<OutputCachingNode>().Any()) return;

            chain.AddToEnd(new OutputCachingNode());
        }

        public static bool ShouldBeCachedPartial(ActionCall call)
        {
            return call.Method.Name.EndsWith(CachedPartial);
        }
    }
}