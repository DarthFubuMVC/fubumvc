using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
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
            var graph = new FubuRegistry(x => x.ApplyHandlerConventions(typeof(Handlers.HandlersMarker))).BuildGraph();
            verifyRoutes(graph);
        }

        [Test]
        public void generic_marker_should_include_handlers()
        {
            var graph = new FubuRegistry(x => x.ApplyHandlerConventions<Handlers.HandlersMarker>()).BuildGraph();
            verifyRoutes(graph);
        }

        [Test]
        public void should_run_in_isolation_from_other_action_matching()
        {
            var graph = new FubuRegistry(registry =>
                                             {
                                                 registry.ApplyHandlerConventions<Handlers.HandlersMarker>();
                                                 registry
                                                     .Actions
                                                     .IncludeType<TestController>();

                                             }).BuildGraph();
            verifyRoutes(graph);
            graph
                .Actions()
                .Where(call => call.HandlerType == typeof (TestController))
                .ShouldHaveCount(6);
        }

        [Test]
        public void should_avoid_duplicates()
        {
            var graph = new FubuRegistry(registry =>
                                             {
                                                 registry.ApplyHandlerConventions<Handlers.HandlersMarker>();
                                                 registry.ApplyHandlerConventions<Handlers.HandlersMarker>();
                                             }).BuildGraph();
            graph.Routes.ShouldHaveCount(5);
        }

        private void verifyRoutes(BehaviorGraph graph)
        {
            var routes = new List<string>
                             {
                                 "posts/create",
                                 "posts/complex-route",
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