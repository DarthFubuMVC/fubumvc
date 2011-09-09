using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Diagnostics.Features.Routes.Authorization;

namespace FubuMVC.Diagnostics.Navigation
{
    public class RouteAuthorizationAction : NavigationItemBase
    {
        public RouteAuthorizationAction(BehaviorGraph graph, IEndpointService endpointService)
            : base(graph, endpointService)
        {
        }

        protected override object inputModel()
        {
            return new AuthorizationRequestModel();
        }
    }
}