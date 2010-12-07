using FubuMVC.Core.Diagnostics.Querying;
using FubuMVC.Core.UI.Diagnostics;

namespace FubuMVC.Core.Diagnostics
{
    public class DiagnosticsRegistry : FubuRegistry
    {
        public DiagnosticsRegistry()
        {
            Actions.IncludeType<BehaviorGraphWriter>();
            Actions.IncludeType<GraphQuery>();
            Actions.IncludeType<ExampleHtmlWriter>();
            Routes.UrlPolicy<DiagnosticUrlPolicy>();
        }
    }
}