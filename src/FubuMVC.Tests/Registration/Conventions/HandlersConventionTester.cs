using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Conventions;
using FubuMVC.Tests.Registration.Conventions.Handlers;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Registration.Conventions
{
    [TestFixture]
    public class HandlersConventionTester
    {
        [Test]
        public void should_include_handlers()
        {
            var graph = BehaviorGraph.BuildFrom(r =>
            {
                r.Import<HandlerConvention>(x => x.MarkerType<HandlersMarker>());
            });

            verifyRoutes(graph);
        }

        [Test]
        public void generic_marker_should_include_handlers()
        {
            var graph = BehaviorGraph.BuildFrom(r =>
            {
                r.Import<HandlerConvention>(x => x.MarkerType<HandlersMarker>());
            });

            verifyRoutes(graph);
        }

        [Test]
        public void should_run_in_isolation_from_other_action_matching()
        {
            var graph = BehaviorGraph.BuildFrom(registry =>
            {
                registry.Import<HandlerConvention>(x => x.MarkerType<HandlersMarker>());
                registry
                    .Actions
                    .IncludeType<TestController>();

            });

            verifyRoutes(graph);
            graph
                .Actions()
                .Where(call => call.HandlerType == typeof (TestController))
                .ShouldHaveCount(6);
        }

        [Test]
        public void should_avoid_duplicates()
        {
            var singleHandlerGraph = BehaviorGraph.BuildFrom(registry =>
            {
                registry.Import<HandlerConvention>(x => x.MarkerType<HandlersMarker>());
            });


            var duplicatedHandlerGraph = BehaviorGraph.BuildFrom(registry =>
            {
                registry.Import<HandlerConvention>(x => x.MarkerType<HandlersMarker>());
                registry.Import<HandlerConvention>(x => x.MarkerType<HandlersMarker>());
            });
                

            duplicatedHandlerGraph.Routes.Count().ShouldEqual(singleHandlerGraph.Routes.Count());
        }

        private void verifyRoutes(BehaviorGraph graph)
        {
            var routes = new List<string>
                             {
                                 "posts/create",
                                 "posts/complex-route",
                                 "posts/sub/route",
                                 "some-crazy-url/as-a-subfolder",
                                 "posts/{Year}/{Month}/{Title}"
                             };

            routes
                .Each(route => graph
                                   .Routes
                                   .ShouldContain(r => r.Pattern.Equals(route)));
        }
    }
}