using FubuMVC.Diagnostics.Models;

namespace FubuMVC.Diagnostics.Endpoints
{
    public class DashboardEndpoint
    {
        [FubuDiagnosticsUrl("~/")]
        public DashboardModel Get(DashboardRequestModel request)
        {
            return new DashboardModel();
        }
    }
}