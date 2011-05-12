using FubuMVC.Core;
using FubuMVC.Core.UI;
using FubuMVC.WebForms;
using FubuTestApplication.ConnegActions;

namespace FubuTestApplication
{
    public class FubuTestApplicationRegistry : FubuRegistry
    {
        public FubuTestApplicationRegistry()
        {
            IncludeDiagnostics(true);

            Import<WebFormsEngine>();

            Actions.IncludeType<ScriptsHandler>();


            Route("conneg/mirror").Calls<MirrorAction>(x => x.Return(null));
            Route("conneg/buckrogers").Calls<MirrorAction>(x => x.BuckRogers());

            Media.ApplyContentNegotiationToActions(call => call.HandlerType == typeof (MirrorAction));
            //this.CombineScriptAndCssFiles(); // only here to test the combining - remove this line if its troublesome
        }
    }
}