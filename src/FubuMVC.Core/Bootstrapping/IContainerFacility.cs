using System;
using System.Collections.Generic;
using System.Web.Routing;
using FubuMVC.Core.Packaging;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Bootstrapping
{
    public interface IContainerFacility
    {
        IBehaviorFactory BuildFactory();
        void Register(Type serviceType, ObjectDef def);
        IEnumerable<IActivator> GetAllActivators();
    }

    public static class ContainerFacilityExtensions
    {
        public static void Activate(this IContainerFacility facility, ICollection<RouteBase> routes, FubuRegistry registry)
        {
            // "Bake" the fubu configuration model into your
            // IoC container for the application
            var graph = registry.BuildGraph();
            graph.EachService(facility.Register);
            var factory = facility.BuildFactory();

            // Register all the Route objects into the routes 
            // collection

            // TODO -- need a way to do this with debugging
            graph.VisitRoutes(x =>
            {
                x.Actions += (routeDef, chain) =>
                {
                    var route = routeDef.ToRoute();
                    route.RouteHandler = new FubuRouteHandler(factory, chain.UniqueId);

                    routes.Add(route);
                };
            });
        }
    }
}