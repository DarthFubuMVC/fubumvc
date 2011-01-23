using System.Collections.Generic;
using System.Web.Routing;
using FubuMVC.Core.Registration;

namespace FubuMVC.Core.Routing
{
    public interface IRoutePolicy
    {
        IList<RouteBase> BuildRoutes(BehaviorGraph graph);
    }
}