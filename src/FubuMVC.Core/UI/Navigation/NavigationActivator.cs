using System;
using System.Collections.Generic;
using Bottles;
using Bottles.Diagnostics;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Querying;

namespace FubuMVC.Core.UI.Navigation
{
    public class NavigationActivator : IActivator
    {
        private readonly IChainResolver _resolver;
        private readonly BehaviorGraph _graph;

        public NavigationActivator(IChainResolver resolver, BehaviorGraph graph)
        {
            _resolver = resolver;
            _graph = graph;
        }

        public void Activate(IEnumerable<IPackageInfo> packages, IPackageLog log)
        {
            log.Trace("Trying to resolve chains to the navigation graph");
            _graph.Navigation.AllNodes().Each(x =>
            {
                try
                {
                    x.Resolve(_resolver);
                }
                catch (Exception ex)
                {
                    log.MarkFailure("Failed to resolve a BehaviorChain for navigation element " + x.Key);
                    log.MarkFailure(ex);
                }

            });
        }
    }
}