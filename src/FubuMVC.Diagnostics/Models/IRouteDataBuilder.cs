using System.Collections.Generic;
using FubuMVC.Core.Registration;

namespace FubuMVC.Diagnostics.Models
{
    public interface IRouteDataBuilder
    {
        IEnumerable<RouteDataModel> BuildRoutes(BehaviorGraph graph);
    }
}