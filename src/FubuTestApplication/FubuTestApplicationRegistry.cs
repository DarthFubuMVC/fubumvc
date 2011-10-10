using FubuMVC.Core;
using FubuMVC.Core.UI;
using FubuMVC.Spark;
using FubuMVC.WebForms;
using FubuTestApplication.ConnegActions;
using FubuCore;

namespace FubuTestApplication
{
    public class FubuTestApplicationRegistry : FubuRegistry
    {
        public FubuTestApplicationRegistry()
        {
            IncludeDiagnostics(true);

            

            Import<WebFormsEngine>();
            this.UseSpark();

            Views
                .TryToAttachViewsInPackages()
                .TryToAttachWithDefaultConventions();


            Actions.IncludeType<ScriptsHandler>();
            Actions.IncludeType<TopPage>();
            Actions.IncludeType<ConnegController>();

            Routes.HomeIs<TopPage>(x => x.Welcome());

            Route("conneg/mirror").Calls<MirrorAction>(x => x.Return(null));
            Route("conneg/buckrogers").Calls<MirrorAction>(x => x.BuckRogers());

            Media.ApplyContentNegotiationToActions(call => call.HandlerType == typeof (MirrorAction))
                .ApplyContentNegotiationToActions(call => call.InputType().CanBeCastTo<ConnegMessage>());
        }
    }
}