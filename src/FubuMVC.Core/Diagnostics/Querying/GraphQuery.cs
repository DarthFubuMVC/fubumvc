using FubuMVC.Core.Content;
using FubuMVC.Core.Registration;
using System.Linq;

namespace FubuMVC.Core.Diagnostics.Querying
{
    public class GraphQuery
    {
        private readonly BehaviorGraph _graph;
        private readonly IContentRegistry _contentRegistry;

        public GraphQuery(BehaviorGraph graph, IContentRegistry contentRegistry)
        {
            _graph = graph;
            _contentRegistry = contentRegistry;
        }

        [UrlPattern("_fubu/all")]
        public EndpointModel All()
        {
            return new EndpointModel(){
                AllEndpoints = _graph.Behaviors.Select(x => new EndpointToken(x)).ToArray()
            };
        }

        [UrlPattern("_fubu/imageurl/{Name}")]
        public string ImageUrlFor(ImageUrlRequest image)
        {
            return _contentRegistry.ImageUrl(image.Name);
        }
    }

    public class ImageUrlRequest
    {
        public string Name { get; set;}
    }
}