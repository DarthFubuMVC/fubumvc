using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Diagnostics.Models;
using FubuMVC.Diagnostics.Models.Routes;

namespace FubuMVC.Diagnostics.Navigation
{
    public class RouteExplorerAction : NavigationItemCaseBase
    {
        public RouteExplorerAction(BehaviorGraph graph, IEndpointService endpointService)
            : base(graph, endpointService)
        {
        }

        protected override object inputModel()
        {
            return new RouteRequestModel();
        }
    }
}