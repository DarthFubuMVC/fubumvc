using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Diagnostics.Features.Dashboard;

namespace FubuMVC.Diagnostics.Navigation
{
    public class DashboardAction : NavigationItemBase
    {
        public DashboardAction(BehaviorGraph graph, IEndpointService endpointService) 
            : base(graph, endpointService)
        {
        }

        protected override object inputModel()
        {
            return new DashboardRequestModel();
        }
    }
}