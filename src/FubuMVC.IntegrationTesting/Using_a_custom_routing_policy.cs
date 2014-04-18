using System.Collections.Generic;
using System.Web.Routing;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Routing;
using FubuMVC.Core.Runtime;
using FubuMVC.StructureMap;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting
{
    [TestFixture]
    public class Using_a_custom_routing_policy
    {
        [Test]
        public void can_happily_plug_in_an_alternative_route_policy()
        {
            FakeRoutePolicy.IWasCalled = false;

            var registry = new FubuRegistry();
            registry.RoutePolicy<FakeRoutePolicy>();

            using (var runtime = FubuApplication.For(registry).StructureMap().Bootstrap())
            {
                
            }

            FakeRoutePolicy.IWasCalled.ShouldBeTrue();
        }
    }

    public class FakeRoutePolicy : IRoutePolicy
    {
        public static bool IWasCalled;

        public IList<RouteBase> BuildRoutes(BehaviorGraph graph, IServiceFactory factory)
        {
            IWasCalled = true;
            return new StandardRoutePolicy().BuildRoutes(graph, factory);
        }
    }
}