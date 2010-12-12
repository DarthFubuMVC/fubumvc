using System.Collections.Generic;
using FubuMVC.Core.Diagnostics.Querying;
using StoryTeller.Assertions;
using StoryTeller.Engine;
using System.Linq;

namespace IntegrationTesting.Fixtures
{
    public class HelloWorldFixture : Fixture
    {
        private readonly RemoteBehaviorGraph _remoteGraph;

        public HelloWorldFixture()
        {
            _remoteGraph = new RemoteBehaviorGraph("http://localhost/helloworld");
        }

        [FormatAs("Route pattern {route} should exist")]
        public bool RouteExists(string route)
        {
            var routeExists = _remoteGraph.All().AllEndpoints.Any(x => x.RoutePattern == route);
            StoryTellerAssert.Fail(!routeExists, () =>
            {
                return "The routes are:  " + _remoteGraph.All().AllEndpoints.Select(x => x.RoutePattern).Join("\n\r");
            });
            
            return routeExists;
        }
    }
}