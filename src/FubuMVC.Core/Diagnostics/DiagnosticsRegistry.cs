using FubuMVC.Core.Diagnostics.Querying;

namespace FubuMVC.Core.Diagnostics
{
    public class DiagnosticsRegistry : FubuRegistry
    {
        public DiagnosticsRegistry()
        {
            Actions.IncludeType<BehaviorGraphWriter>();
            Actions.IncludeType<GraphQuery>();
            Routes.UrlPolicy<DiagnosticUrlPolicy>();
        }
    }
}