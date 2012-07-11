using FubuMVC.Core.UI.Navigation;
using FubuMVC.Diagnostics.Features.Dashboard;
using FubuMVC.Diagnostics.Features.Html;
using FubuMVC.Diagnostics.Features.Packaging;

namespace FubuMVC.Diagnostics.Navigation
{
    public class DiagnosticsMenu : NavigationRegistry
    {
        public DiagnosticsMenu()
        {
            ForMenu(DiagnosticKeys.Main);
            Add += MenuNode.ForInput<DashboardRequestModel>(DiagnosticKeys.Dashboard);
            Add += MenuNode.ForInput<HtmlConventionsRequestModel>(DiagnosticKeys.HtmlConventions);
            Add += MenuNode.ForInput<PackageDiagnosticsRequestModel>(DiagnosticKeys.ApplicationStartup);

            // TODO -- add it back in!
            //Add += MenuNode.ForInput<RequestExplorerRequestModel>(DiagnosticKeys.Requests);
            //Add += MenuNode.ForInput<DefaultRouteRequestModel>(DiagnosticKeys.Routes);
        }
    }

    // TODO -- add tests
}