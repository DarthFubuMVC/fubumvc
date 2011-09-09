using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Diagnostics.Features.Routes;

namespace FubuMVC.Diagnostics.Navigation
{
    public class RouteExplorerAction : NavigationItemBase
    {
        public RouteExplorerAction(BehaviorGraph graph, IEndpointService endpointService)
            : base(graph, endpointService)
        {
        }

        public override string Text()
        {
            return "Route Explorer";
        }

        protected override object inputModel()
        {
            return new DefaultRouteRequestModel();
        }
    }
}