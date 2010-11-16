using System.Collections.Generic;
using System.Web;
using System.Web.Routing;
using FubuCore;
using FubuMVC.Core.Bootstrapping;
using FubuMVC.Core.Packaging;
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

            // Find all of the IFubuRegistryExtension's and apply
            // them to the top level FubuRegistry *BEFORE*
            // registering the Fubu application parts into
            // your IoC container
            PackageLoader.FindAllExtensions().Each(x => x.Configure(_topRegistry));

            // "Bake" the fubu configuration model into your
            // IoC container for the application
            BehaviorGraph graph = _topRegistry.BuildGraph();
            graph.EachService(_facility.Register);
            IBehaviorFactory factory = _facility.BuildFactory();

            // Register all the Route objects into the routes 
            // collection

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