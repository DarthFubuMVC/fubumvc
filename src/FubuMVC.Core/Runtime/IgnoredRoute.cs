using System.Web.Routing;

namespace FubuMVC.Core.Runtime
{
    public class IgnoredRoute : Route
    {
        public IgnoredRoute(string url)
            : base(url, new StopRoutingHandler())
        {
        }

        public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary routeValues)
        {
            return null;
        }
    }
}