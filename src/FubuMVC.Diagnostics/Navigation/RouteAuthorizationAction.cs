using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Diagnostics.Models.Routes;

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