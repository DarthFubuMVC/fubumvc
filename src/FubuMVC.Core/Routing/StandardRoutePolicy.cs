using System.Collections.Generic;
using System.Web.Routing;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Routing
{
    public class StandardRoutePolicy : IRoutePolicy
    {
        public IList<RouteBase> BuildRoutes(BehaviorGraph graph, IBehaviorFactory factory)
        {
            var routes = new List<RouteBase>();
            graph.VisitRoutes(x =>
            {
                x.BehaviorFilters += chain => !chain.IsPartialOnly;
                x.Actions += (routeDef, chain) =>
                {
                    var route = routeDef.ToRoute();
                    route.RouteHandler = new FubuRouteHandler(factory, chain.UniqueId);
                    routes.Add(route);
                };
            });
            return routes;
        }
    }
}