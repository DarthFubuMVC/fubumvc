namespace FubuMVC.Core.Diagnostics
{
    public class DiagnosticsRegistry : FubuRegistry
    {
        public DiagnosticsRegistry()
        {
            Actions.IncludeType<BehaviorGraphWriter>();
            Routes.UrlPolicy<DiagnosticUrlPolicy>();
        }
    }
}