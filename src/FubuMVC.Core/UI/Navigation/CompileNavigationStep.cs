using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Querying;

namespace FubuMVC.Core.UI.Navigation
{
    // Depending on integration tests for this one.
    public class CompileNavigationStep : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            graph.Navigation.Compile();

            var resolver = new ChainResolutionCache(new TypeResolver(), graph);

            graph.Navigation.AllNodes().OfType<MenuNode>().Each(x =>
            {
                try
                {
                    x.Resolve(resolver);
                }
                catch (Exception ex)
                {
                    throw new FubuException(4001, ex, "Failed to resolve a BehaviorChain for navigation element " + x.Key);
                }

            });
        }
    }
}