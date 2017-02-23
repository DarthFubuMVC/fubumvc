using System;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Routes;
using Shouldly;
using Xunit;

namespace FubuMVC.Tests
{
    
    public class UrlAliasAttributeTester
    {
        private BehaviorGraph theGraph;

        public UrlAliasAttributeTester()
        {
            var registry = new FubuRegistry();
            registry.Actions.IncludeType<RouteAliasController>();

            theGraph = BehaviorGraph.BuildFrom(registry);
        }

        [Fact]
        public void adds_the_route_aliases()
        {
            var route1 = new RouteDefinition("something/else");
            var route2 = new RouteDefinition("again/something/else");

            var chain = theGraph.ChainFor<RouteAliasController>(x => x.get_something())
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