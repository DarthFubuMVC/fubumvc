using System;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Routes;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests
{
    [TestFixture]
    public class UrlAliasAttributeTester
    {
        private BehaviorGraph theGraph;

        [SetUp]
        public void SetUp()
        {
            var registry = new FubuRegistry();
            registry.Actions.IncludeType<RouteAliasController>();

            theGraph = BehaviorGraph.BuildFrom(registry);
        }

        [Test]
        public void adds_the_route_aliases()
        {
            var route1 = new RouteDefinition("something/else");
            var route2 = new RouteDefinition("again/something/else");

            var chain = theGraph.BehaviorFor<RouteAliasController>(x => x.get_something())
                .As<RoutedChain>();
            var routes = chain.AdditionalRoutes;

            routes.ShouldHaveCount(2);
            routes.ShouldContain(route1);
            routes.ShouldContain(route2);
        }

        public class RouteAliasController
        {
            [UrlAlias("something/else")]
            [UrlAlias("again/something/else")]
            public string get_something()
            {
                throw new NotImplementedException();
            }
        }
    }
}