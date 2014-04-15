using System.Collections.Generic;
using FubuMVC.Core.Http;
using FubuMVC.Core.Projections;
using FubuMVC.Core.Registration.Querying;

namespace FubuMVC.Core.Assets.JavascriptRouting
{
    // Tested by integration tests only
    public class JavascriptRouteWriter
    {
        private readonly IHttpRequest _request;
        private readonly IChainResolver _resolver;

        public JavascriptRouteWriter(IHttpRequest request, IChainResolver resolver)
        {
            _request = request;
            _resolver = resolver;
        }

        public IDictionary<string, object> Write(IEnumerable<JavascriptRoute> routes)
        {
            var node = new DictionaryMediaNode();

            routes.Each(x => {
                var child = node.AddChild(x.Name);
                x.WriteNode(child, _request, _resolver);
            });

            return node.Values;
        }
    }
}