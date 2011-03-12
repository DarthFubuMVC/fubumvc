using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Diagnostics.Models;

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