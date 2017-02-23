using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Querying;
using FubuMVC.Core.Registration.Routes;
using Shouldly;
using Xunit;
using Rhino.Mocks;

namespace FubuMVC.Tests.Registration.Querying
{
    
    public class ChainResoluationCacheRootAt 
    {
        private ChainResolutionCache theCache;
        private RouteDefinition theRouteDefinitionAlias;

        public ChainResoluationCacheRootAt()
        {
            theCache = new ChainResolutionCache(setupActions());
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

                x.ChainFor<RouteAliasAction1>(_ => _.M1()).As<RoutedChain>().AddRouteAlias(theRouteDefinitionAlias);
            });

            return BehaviorGraph.BuildFrom(registry);
        }

        [Fact]
        public void roots_route_aliases()
        {
            theCache.RootAt("localhost");
            var route = theRouteDefinitionAlias.ToRoute();
            route.Url.ShouldBe("localhost/{Client}/");
        }
    }

    public class RouteAliasAction1
    {
        [UrlAlias("prefixed/a/m1")]
        public void M1(){}
        public void M2(){}
    }
}