using FubuMVC.Core.Diagnostics;
using FubuMVC.Diagnostics.Endpoints;

namespace FubuMVC.Diagnostics
{
    public class CoreDiagnosticsGroup : DiagnosticGroup
    {
        public CoreDiagnosticsGroup() : base("core")
        {
            Endpoint<EndpointExplorerFubuDiagnostics>("fubumvc.endpoints", x => x.get_endpoints(null));

            Stylesheets.Add("diagnostics/bootstrap.overrides.css");
            Scripts.Add("diagnostics/core-diagnostics.js");
            ReactFiles.Add("diagnostics/navigation-react.js");
        }
    }
}