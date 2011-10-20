using System.Linq;
using FubuMVC.Core.Assets;
using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Assets.Http;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Urls;

namespace FubuMVC.Core.Diagnostics.Querying
{
    public class GraphQuery
    {
        private readonly BehaviorGraph _graph;
        private readonly IUrlRegistry _urls;

        public GraphQuery(BehaviorGraph graph, IUrlRegistry urls)
        {
            _graph = graph;
            _urls = urls;
        }

        [UrlPattern("_fubu/all")]
        public EndpointModel All()
        {
            return new EndpointModel{
                AllEndpoints = _graph.Behaviors.Select(x => new EndpointToken(x)).ToArray()
            };
        }

        [UrlPattern("_fubu/imageurl/{Name}")]
        public string ImageUrlFor(ImageUrlRequest image)
        {
            return _urls.UrlForAsset(AssetFolder.images, image.Name);
        }
    }

    public class ImageUrlRequest
    {
        public string Name { get; set; }
    }
}