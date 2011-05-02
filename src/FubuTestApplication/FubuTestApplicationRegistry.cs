using FubuFastPack.JqGrid;
using FubuMVC.Core;
using FubuMVC.WebForms;
using FubuTestApplication.ConnegActions;
using FubuTestApplication.Grids;

namespace FubuTestApplication
{
    public class FubuTestApplicationRegistry : FubuRegistry
    {
        public FubuTestApplicationRegistry()
        {
            IncludeDiagnostics(true);

            Import<WebFormsEngine>();

            Actions.IncludeType<ScriptsHandler>();

            Route("cases").Calls<CaseController>(x => x.AllCases());
            Route("case/{Id}").Calls<CaseController>(x => x.Show(null));
            Route("viewcase/{Identifier}").Calls<CaseController>(x => x.Case(null));
            Route("loadcases").Calls<CaseController>(x => x.LoadCases(null));
            Route("person/{Id}").Calls<CaseController>(x => x.Person(null));

            Route("conneg/mirror").Calls<MirrorAction>(x => x.Return(null));
            Route("conneg/buckrogers").Calls<MirrorAction>(x => x.BuckRogers());

            Media.ApplyContentNegotiationToActions(call => call.HandlerType == typeof (MirrorAction));

            this.ApplySmartGridConventions(x => { x.ToThisAssembly(); });
        }
    }
}