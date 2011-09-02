using System.Collections.Generic;
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