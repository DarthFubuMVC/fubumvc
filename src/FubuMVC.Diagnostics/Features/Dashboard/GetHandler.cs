namespace FubuMVC.Diagnostics.Features.Dashboard
{
    public class GetHandler
    {
        [FubuDiagnosticsUrl("~/")]
        public DashboardModel Execute(DashboardRequestModel request)
        {
            return new DashboardModel();
        }
    }
}