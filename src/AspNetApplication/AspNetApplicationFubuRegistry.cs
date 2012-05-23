using AspNetApplication.ServerSideEvents;
using FubuMVC.Core;
using FubuMVC.Core.Diagnostics.HtmlWriting;

namespace AspNetApplication
{
    public class AspNetApplicationFubuRegistry : FubuRegistry
    {
        public AspNetApplicationFubuRegistry()
        {
            Actions.IncludeClassesSuffixedWithController();
            IncludeDiagnostics(true);

            Routes.HomeIs<SSEClientController>(x => x.get_events());
        }
    }
}