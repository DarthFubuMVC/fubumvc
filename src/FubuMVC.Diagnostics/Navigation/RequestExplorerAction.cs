using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Diagnostics.Features.Requests;

namespace FubuMVC.Diagnostics.Navigation
{
    [MarkedForTermination]
    public class RequestExplorerAction : NavigationItemBase
    {
        public RequestExplorerAction(BehaviorGraph graph, IEndpointService endpointService)
            : base(graph, endpointService)
        {
        }

        public override string Text()
        {
            return "Requests";
        }

        protected override object inputModel()
        {
            return new RequestExplorerRequestModel();
        }
    }
}