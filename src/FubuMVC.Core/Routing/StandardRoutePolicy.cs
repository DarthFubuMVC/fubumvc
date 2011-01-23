using System.Collections.Generic;
using System.Web.Routing;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Routing
{
    public class StandardRoutePolicy : IRoutePolicy
    {
        private readonly IBehaviorFactory _factory;
        public StandardRoutePolicy(IBehaviorFactory factory)
        {
            _factory = factory;
        }

        public IList<RouteBase> BuildRoutes(BehaviorGraph graph)
        {
            var routes = new List<RouteBase>();
            graph.VisitRoutes(x =>
            {
                x.Actions += (routeDef, chain) =>
                {
                    var route = routeDef.ToRoute();
                    route.RouteHandler = new FubuRouteHandler(_factory, chain.UniqueId);
                    routes.Add(route);
                };
            });
            return routes;
        }
    }
}