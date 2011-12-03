using System.Collections.Generic;
using System.Linq;
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
                    if(chain.Calls.Any(c => c.IsAsync))
                        route.RouteHandler = new FubuAsyncRouteHandler(new AsyncBehaviorInvoker(factory, chain));
                    else
                        route.RouteHandler = new FubuRouteHandler(new BehaviorInvoker(factory, chain));
                    routes.Add(route);
                };
            });
            return routes;
        }
    }
}