using System.Diagnostics;
using FubuCore;
using FubuMVC.Core.Http.Hosting;
using FubuMVC.Core.Security.Authentication;
using FubuMVC.Core.Security.Authentication.Endpoints;
using FubuMVC.RavenDb.Membership;
using FubuMVC.RavenDb.Reset;
using Serenity;
using ServiceNode;
using StoryTeller.Engine;
using StructureMap;
using WebsiteNode;

namespace FubuMVC.IntegrationTesting
{
    public class TestSystem : SerenitySystem<WebsiteRegistry>
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

            AddRemoteSubSystem("ServiceNode", x =>
            {
                x.UseParallelServiceDirectory("ServiceNode");
                x.Setup.ShadowCopyFiles = false.ToString();
            });
        }

        protected override void beforeEach(IContainer scope)
        {
            TextFileWriter.Clear();

            Runtime.Get<MessageRecorder>().Messages.Clear();

            var browser = scope.GetInstance<IBrowserLifecycle>();
            if (browser.HasBeenStarted())
            {
                scope.GetInstance<NavigationDriver>().NavigateTo<LogoutRequest>();
            }

            Runtime.Get<ICompleteReset>().ResetState();
        }
    }
}