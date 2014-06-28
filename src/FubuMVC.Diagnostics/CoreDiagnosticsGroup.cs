using System.Security.Cryptography.X509Certificates;
using FubuMVC.Core;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Diagnostics.Endpoints;

namespace FubuMVC.Diagnostics
{
    public class CoreDiagnosticsGroup : DiagnosticGroup
    {
        public CoreDiagnosticsGroup() : base("core")
        {
            Endpoint<EndpointExplorerFubuDiagnostics>("fubumvc.endpoints", x => x.get_endpoints(null));
        }
    }
}