using System.Collections.Generic;
using System.Web;
using System.Web.Routing;
using FubuCore;
using FubuMVC.Core.Bootstrapping;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.View;

namespace FubuMVC.Core
{
    public class FubuBootstrapper
    {
        private readonly IContainerFacility _facility;
        private readonly FubuRegistry _topRegistry;

        public FubuBootstrapper(IContainerFacility facility, FubuRegistry topRegistry)
        {
            _facility = facility;
            _topRegistry = topRegistry;
        }

        public void Bootstrap(ICollection<RouteBase> routes)
        {
            if (HttpContext.Current != null)
            {
                UrlContext.Live();
            }


            BehaviorGraph graph = _topRegistry.BuildGraph();
            graph.EachService(_facility.Register);
            IBehaviorFactory factory = _facility.BuildFactory();

            // TODO -- need a way to do this with debugging
            graph.VisitRoutes(x =>
            {
                x.Actions += (routeDef, chain) =>
                {
                    Route route = routeDef.ToRoute();
                    route.RouteHandler = new FubuRouteHandler(factory, chain.UniqueId);

                    routes.Add(route);
                };
            });
        }

    }
}