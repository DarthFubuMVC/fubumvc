using FubuMVC.Core;

namespace FubuMVC.UI.Diagnostics
{
    public class HtmlDiagnosticsRegistry : FubuRegistry
    {
        public HtmlDiagnosticsRegistry()
        {
            Actions.IncludeType<ExampleHtmlWriter>();
        }
    }
}