using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Http;
using FubuMVC.Core.Projections;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Querying;
using FubuMVC.Core.Registration.Routes;

namespace FubuMVC.Core.Assets.JavascriptRouting
{
    // Tested by integration tests only
    public class JavascriptRouteWriter
    {
        private readonly IJavascriptRouteData _routeData;
        private readonly IChainResolver _resolver;

        public JavascriptRouteWriter(IJavascriptRouteData routeData, IChainResolver resolver)
        {
            _routeData = routeData;
            _resolver = resolver;
        }

        public IDictionary<string, object> Write(IEnumerable<JavascriptRoute> routes)
        {
            var node = new DictionaryMediaNode();

            routes.Each(x => {
                var child = node.AddChild(x.Name);
                var chain = x.FindChain(_resolver);
                child.SetAttribute("name", x.Name);
                child.SetAttribute("method", x.Method);

                child.SetAttribute("url", _routeData.ToUrl(chain));

                var parameters = _routeData.ToParameters(chain);
                if (parameters.Any())
                {
                    child.SetAttribute("params", parameters);
                }
                


            });

            return node.Values;
        }
    }

    public class JavascriptRouteData : IJavascriptRouteData
    {
        private readonly IHttpRequest _request;

        public JavascriptRouteData(IHttpRequest request)
        {
            _request = request;
        }

        public string ToUrl(RoutedChain chain)
        {
            return _request.ToFullUrl(chain.GetRoutePattern());
        }

        public IEnumerable<RouteParameter> ToParameters(RoutedChain chain)
        {
            if (chain.Route.Input == null)
            {
                return Enumerable.Empty<RouteParameter>();
            }

            return chain.Route.Input.RouteParameters;
        }
    }

    public interface IJavascriptRouteData
    {
        string ToUrl(RoutedChain chain);
        IEnumerable<RouteParameter> ToParameters(RoutedChain chain);
    }
}