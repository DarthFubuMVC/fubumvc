using FubuMVC.Core;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.UI;
using FubuMVC.Spark;
using FubuMVC.WebForms;
using FubuTestApplication.ConnegActions;
using FubuCore;
using FubuMVC.Core.Resources.Conneg;
using FubuTestApplication.CurrentRequest;

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

            Route("currentrequest/get").Calls<CurrentRequestAction>(x => x.Get(null));

            Media.ApplyContentNegotiationToActions(call => call.HandlerType == typeof (MirrorAction))
                .ApplyContentNegotiationToActions(call => call.InputType().CanBeCastTo<ConnegMessage>());

            Configure(graph =>
            {
                graph.BehaviorFor<ConnegController>(x => x.Mixed(null)).AddToEnd(Wrapper.For<ConnegMessageOutputter>());

                var chain = graph.BehaviorFor<ConnegController>(x => x.FormatterOnly(null));
                chain.ApplyConneg();
                chain.Input.AllowHttpFormPosts = false;

                chain.AddToEnd(Wrapper.For<ConnegMessageOutputter>());
            });
        }
    }
}