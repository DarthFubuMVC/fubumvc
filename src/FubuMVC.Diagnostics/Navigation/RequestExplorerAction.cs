using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Diagnostics.Features.Requests;

namespace FubuMVC.Diagnostics.Navigation
{
    public class RequestExplorerAction : NavigationItemBase
    {
        public RequestExplorerAction(BehaviorGraph graph, IEndpointService endpointService)
            : base(graph, endpointService)
        {
        }

        public override string Text()
        {
            return "Request Explorer";
        }

        protected override object inputModel()
        {
            return new RequestExplorerRequestModel();
        }
    }
}