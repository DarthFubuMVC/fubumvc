using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Runtime.Handlers;

namespace FubuMVC.Core.Registration.Routes
{
    

    public class StandardRoutePolicy : IRoutePolicy
    {
        public IList<RouteBase> BuildRoutes(BehaviorGraph graph, IServiceFactory factory)
        {
            var defaultSessionRequirement = graph.Settings.Get<SessionStateRequirement>();
            var orderedEnumerable = graph
                .Chains.OfType<RoutedChain>()
                .SelectMany(toRoutes)
                .OrderBy(x => x.Route.Rank);

            return orderedEnumerable
                .Select(x => buildRoute(factory, defaultSessionRequirement, x.Chain, x.Route))
                .ToList();
        }

        public class ChainRoute
        {
            public RoutedChain Chain;
            public IRouteDefinition Route;

            public override string ToString()
            {
                return string.Format("Chain: {0}, Route: {1}", Chain, Route.Pattern);
            }
        }


        private IEnumerable<ChainRoute> toRoutes(RoutedChain chain)
        {
            yield return new ChainRoute {Chain = chain, Route = chain.Route};

            foreach (var additionalRoute in chain.AdditionalRoutes)
            {
                yield return new ChainRoute {Chain = chain, Route = additionalRoute};
            }
        }


        private RouteBase buildRoute(IServiceFactory factory, SessionStateRequirement defaultSessionRequirement,
                                    BehaviorChain chain, IRouteDefinition routeDefinition)
        {
            var requiresSession = routeDefinition.SessionStateRequirement ?? defaultSessionRequirement;

            var route = routeDefinition.ToRoute();
            var handlerSource = DetermineHandlerSource(requiresSession, chain);
            var invoker = DetermineInvoker(factory, chain);

            route.RouteHandler = new FubuRouteHandler(invoker, handlerSource);

            return route;
        }

        public static IBehaviorInvoker DetermineInvoker(IServiceFactory factory, BehaviorChain chain)
        {
            return new BehaviorInvoker(factory, chain);
        }

        public static IHttpHandlerSource DetermineHandlerSource(SessionStateRequirement sessionStateRequirement, BehaviorChain chain)
        {
            if (chain.IsAsynchronous())
            {
                return sessionStateRequirement == SessionStateRequirement.RequiresSessionState
                           ? (IHttpHandlerSource) new AsynchronousHttpHandlerSource()
                           : new SessionlessAsynchronousHttpHandlerSource();
            }

            return sessionStateRequirement == SessionStateRequirement.RequiresSessionState
                       ? (IHttpHandlerSource)new SynchronousHttpHandlerSource()
                       : new SessionlessSynchronousHttpHandlerSource();
        }
    }
}