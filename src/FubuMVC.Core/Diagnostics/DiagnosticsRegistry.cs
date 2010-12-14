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

            Actions.IncludeMethods(x => x.HasAttribute<FubuDiagnosticsAttribute>());
            Routes.UrlPolicy<DiagnosticUrlPolicy>();
        }
    }
}