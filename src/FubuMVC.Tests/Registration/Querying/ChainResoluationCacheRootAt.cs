using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Querying;
using FubuMVC.Core.Registration.Routes;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Registration.Querying
{
    [TestFixture]
    public class ChainResoluationCacheRootAt 
    {
        private ChainResolutionCache theCache;
        private RouteDefinition theRouteDefinitionAlias;

        [SetUp]
        public void SetUp()
        {
            theCache = new ChainResolutionCache(new TypeResolver(), setupActions());
        }

        private BehaviorGraph setupActions()
        {
            var registry = new FubuRegistry();
            registry.Actions.IncludeType<RouteAliasAction1>();

            registry.Configure(x =>
            {
                theRouteDefinitionAlias = new RouteDefinition("{Client}/");
                theRouteDefinitionAlias.Input = MockRepository.GenerateMock<IRouteInput>();
                theRouteDefinitionAlias.Input.Stub(_ => _.Rank).Return(5);

                x.BehaviorFor<RouteAliasAction1>(_ => _.M1()).As<RoutedChain>().AddRouteAlias(theRouteDefinitionAlias);
            });

            return BehaviorGraph.BuildFrom(registry);
        }

        [Test]
        public void roots_route_aliases()
        {
            theCache.RootAt("localhost");
            var route = theRouteDefinitionAlias.ToRoute();
            route.Url.ShouldEqual("localhost/{Client}/");
        }
    }

    public class RouteAliasAction1
    {
        [UrlAlias("prefixed/a/m1")]
        public void M1(){}
        public void M2(){}
    }
}