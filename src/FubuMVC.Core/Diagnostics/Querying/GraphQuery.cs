using FubuMVC.Core.Registration;
using System.Linq;

namespace FubuMVC.Core.Diagnostics.Querying
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
            return new EndpointModel(){
                AllEndpoints = _graph.Behaviors.Select(x => new EndpointToken(x)).ToArray()
            };
        }
    }
}