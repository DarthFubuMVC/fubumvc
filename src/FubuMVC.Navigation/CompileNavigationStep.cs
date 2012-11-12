using System;
using System.Linq;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Querying;
using System.Collections.Generic;

namespace FubuMVC.Navigation
{
    // Depending on integration tests for this one.
    [ConfigurationType(ConfigurationType.Navigation)]
    public class CompileNavigationStep : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            var navigation = graph.Settings.Get<NavigationGraph>();

            navigation.Compile();

            var resolver = new ChainResolutionCache(new TypeResolver(), graph);

            navigation.AllNodes().OfType<MenuNode>().Each(x =>
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