using System.Collections.Generic;
using System.Web.Routing;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Registration.Routes
{
    public interface IRoutePolicy
    {
        IList<RouteBase> BuildRoutes(BehaviorGraph graph, IServiceFactory factory);
    }
}