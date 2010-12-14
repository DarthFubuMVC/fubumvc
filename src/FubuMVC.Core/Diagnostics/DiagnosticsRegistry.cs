using FubuMVC.Core.Diagnostics.Querying;
using FubuMVC.Core.UI.Diagnostics;
using FubuCore.Reflection;

namespace FubuMVC.Core.Diagnostics
{
    public class DiagnosticsRegistry : FubuRegistry
    {
        public DiagnosticsRegistry()
        {
            Applies.ToAssemblyContainingType<DiagnosticsRegistry>();

            Actions.IncludeTypes(x => x.HasAttribute<FubuDiagnosticsAttribute>()).IncludeType<GraphQuery>();
            Routes.UrlPolicy<DiagnosticUrlPolicy>();
        }
    }
}