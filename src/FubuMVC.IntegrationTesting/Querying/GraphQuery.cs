using System.Linq;
using FubuMVC.Core;
using FubuMVC.Core.Registration;

namespace FubuMVC.IntegrationTesting.Querying
{
    public class GraphQuery
    {
        private readonly BehaviorGraph _graph;

        public GraphQuery(BehaviorGraph graph)
        {
            _graph = graph;
        }

        [UrlPattern("_fubu/all")]
        public EndpointModel All()
        {
            return new EndpointModel
            {
                AllEndpoints = _graph.Behaviors.Select(x => new EndpointToken(x)).ToArray()
            };
        }
    }

    public class ImageUrlRequest
    {
        public string Name { get; set; }
    }
}