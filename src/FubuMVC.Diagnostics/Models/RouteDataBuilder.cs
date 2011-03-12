using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Registration;

namespace FubuMVC.Diagnostics.Models
{
    public interface IRouteDataBuilder
    {
        IEnumerable<RouteDataModel> BuildRoutes(BehaviorGraph graph);
    }

    public class RouteDataBuilder : IRouteDataBuilder
    {
        public IEnumerable<RouteDataModel> BuildRoutes(BehaviorGraph graph)
        {
            return graph
                .Behaviors
                .Select(b => new RouteDataModel
                                 {
                                     Id = b.UniqueId.ToString(),
                                     Route = b.RoutePattern,
                                     Constraints = "pending",
                                     Action = b.FirstCallDescription,
                                     InputModel = "pending",
                                     OutputModel = "pending"                                     
                                 });
        }
    }
}