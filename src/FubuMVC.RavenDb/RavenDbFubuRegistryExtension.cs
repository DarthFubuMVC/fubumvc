using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.RavenDb.RavenDb;

namespace FubuMVC.RavenDb
{
    public class RavenDbFubuRegistryExtension : IFubuRegistryExtension
    {
        public void Configure(FubuRegistry registry)
        {
            registry.Services.IncludeRegistry<RavenDbRegistry>();
        }
    }

    public class TransactionalBehaviorPolicy : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            graph.Chains.OfType<RoutedChain>()
                .Where(
                    x =>
                        x.Route.RespondsToMethod("POST") || x.Route.RespondsToMethod("PUT") ||
                        x.Route.RespondsToMethod("DELETE"))
                .Each(x => x.InsertFirst(Wrapper.For<TransactionalBehavior>()));
        }
    }
}