namespace FubuMVC.Core.UI.Diagnostics
{
    public class HtmlDiagnosticsRegistry : FubuRegistry
    {
        public HtmlDiagnosticsRegistry()
        {
            Actions.IncludeType<ExampleHtmlWriter>();
        }
    }
}