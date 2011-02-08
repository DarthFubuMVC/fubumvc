using System.Collections.Generic;
using System.Web.Routing;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Routing
{
    public interface IRoutePolicy
    {
        IList<RouteBase> BuildRoutes(BehaviorGraph graph, IBehaviorFactory factory);
    }
}