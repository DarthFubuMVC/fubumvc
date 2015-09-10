using FubuCore;
using FubuCore.Dates;
using FubuMVC.Core.Http.Hosting;
using FubuMVC.Core.Security.Authentication;
using FubuMVC.Core.Security.Authentication.Endpoints;
using FubuMVC.RavenDb.Membership;
using FubuMVC.RavenDb.Reset;
using Serenity;
using StructureMap;

namespace FubuMVC.IntegrationTesting
{
    public class TestSystem : SerenitySystem
    {
        public TestSystem()
        {
            Registry.Import<PersistedMembership<User>>();
            Registry.Features.Authentication.Configure(_ =>
            {
                _.ExcludeDiagnostics = true;
                _.Enabled = true;
            });

            Registry.HostWith<Katana>(5555);

            Registry.Configure(
                graph =>
                {
                    graph.ChainFor<LoginController>(x => x.get_login(null)).Output.Add(new DefaultLoginRequestWriter());
                });
        }

        protected override void beforeEach(IContainer scope)
        {
            var browser = scope.GetInstance<IBrowserLifecycle>();
            if (browser.HasBeenStarted())
            {
                scope.GetInstance<NavigationDriver>().NavigateTo<LogoutRequest>();
            }

            Runtime.Get<ICompleteReset>().ResetState();
        }

        /*
         * 
         * 
            AddRemoteSubSystem("ServiceNode", x => {
                x.UseParallelServiceDirectory("ServiceNode");
                x.Setup.ShadowCopyFiles = false.ToString();
            });

            

            OnContextCreation(TextFileWriter.Clear);

            OnContextCreation<WebsiteNode.MessageRecorder>(x => x.Messages.Clear());
         * 
         * 
         */
    }
}