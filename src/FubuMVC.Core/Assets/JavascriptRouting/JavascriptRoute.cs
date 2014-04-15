using System;
using System.Linq;
using FubuMVC.Core.Http;
using FubuMVC.Core.Projections;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Querying;

namespace FubuMVC.Core.Assets.JavascriptRouting
{
    public class JavascriptRoute
    {
        public string Name;
        public string Method;
        public Func<IChainResolver, BehaviorChain> Finder;

        public void WriteNode(IMediaNode node, IHttpRequest request, IChainResolver resolver)
        {
            var chain = Finder(resolver) as RoutedChain;

            if (chain == null)
            {
                throw new Exception("Unable to find a routed chain for a Javascript route named " + Name);
            }

            node.SetAttribute("name", Name);
            node.SetAttribute("method", Method);
            node.SetAttribute("url", request.ToFullUrl(chain.GetRoutePattern()));
            if (chain.Route.Input != null)
                node.SetAttribute("params", chain.Route.Input.RouteParameters.Select(x => x.Name).ToArray());
        }
    }
}