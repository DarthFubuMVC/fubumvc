using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Http.Hosting;
using FubuMVC.Core.Security.Authentication;
using FubuMVC.Core.Security.Authentication.Endpoints;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.Polling;
using FubuMVC.IntegrationTesting.Fixtures.ServiceBus.Basic;
using FubuMVC.RavenDb.Membership;
using FubuMVC.RavenDb.Reset;
using Serenity;
using ServiceNode;
using Shouldly;
using StoryTeller;
using StoryTeller.Engine;
using StoryTeller.Model.Lists;
using StructureMap;

namespace FubuMVC.IntegrationTesting
{
    public class WebsiteRegistry : FubuTransportRegistry<TestBusSettings>
    {
        public WebsiteRegistry()
        {
            Channel(x => x.Website).ReadIncoming();
            Channel(x => x.Service).AcceptsMessagesInAssemblyContainingType<ServiceRegistry>();

            ServiceBus.HealthMonitoring.ScheduledExecution(ScheduledExecution.Disabled);

            Import<PersistedMembership<User>>();

            Features.Authentication.Configure(_ =>
            {
                _.ExcludeDiagnostics = true;
                _.Enabled = true;
            });

            HostWith<Katana>(5555);

            Configure(
                graph =>
                {
                    graph.ChainFor<LoginController>(x => x.get_login(null)).Output.Add(new DefaultLoginRequestWriter());
                });
        }
    }

    public class MessageRecorder
    {
        public IList<string> Messages = new List<string>();
    }

    public class TestSystem : SerenitySystem<WebsiteRegistry>
    {
        public static void TryIt()
        {
            using (var runner = new StoryTeller.SpecRunner<TestSystem>())
            {
                runner.Run("ServiceBus/Basics/Sending a message that results in replies");
                runner.OpenResultsInBrowser();
            }
        }

        public TestSystem()
        {
            AddRemoteSubSystem("ServiceNode", x =>
            {
                x.UseParallelServiceDirectory("ServiceNode");
                x.Setup.ShadowCopyFiles = false.ToString();
            });
        }

        protected override void beforeEach(IContainer scope)
        {
            Retry.Twice(() => TextFileWriter.Clear());

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