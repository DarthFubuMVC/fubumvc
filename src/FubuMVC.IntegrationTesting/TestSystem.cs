using System.Collections.Generic;
using FubuMVC.Core.Http.Hosting;
using FubuMVC.Core.Security.Authentication;
using FubuMVC.Core.Security.Authentication.Endpoints;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.Polling;
using FubuMVC.RavenDb.Membership;
using FubuMVC.RavenDb.Reset;
using NUnit.Framework;
using Serenity;
using ServiceNode;
using StoryTeller;
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

    public class StorytellerRunner
    {
        [Test]
        public void run_a_single_test()
        {
            using (var runner = new SpecRunner<TestSystem>())
            {
                runner.Run("ServiceBus/Basics/Publishing a message that has multiple subscribers");

                //runner.Run("ServiceBus/HealthMonitoring/A running task goes down and gets reassigned");
                //runner.Run("ServiceBus/HealthMonitoring/Assign on order of preference when some nodes are down");
                //runner.Run("ServiceBus/HealthMonitoring/Assign on order or preference when some nodes timeout on activation");
                //runner.Run("ServiceBus/HealthMonitoring/A running task times out on health checks and gets reassigned");
                runner.OpenResultsInBrowser();
            }
        }
    }

    public class TestSystem : SerenitySystem<WebsiteRegistry>
    {
        public TestSystem()
        {
            AddRemoteSubSystem("ServiceNode", x =>
            {
                x.UseParallelServiceDirectory("ServiceNode");
                x.Setup.ShadowCopyFiles = false.ToString();
            });
        }

        public static void TryIt()
        {
            using (var runner = new SpecRunner<TestSystem>())
            {
                runner.Run("ServiceBus/Basics/Publishing a message that has multiple subscribers");

                //runner.Run("ServiceBus/HealthMonitoring/A running task goes down and gets reassigned");
                //runner.Run("ServiceBus/HealthMonitoring/Assign on order of preference when some nodes are down");
                //runner.Run("ServiceBus/HealthMonitoring/Assign on order or preference when some nodes timeout on activation");
                //runner.Run("ServiceBus/HealthMonitoring/A running task times out on health checks and gets reassigned");
                runner.OpenResultsInBrowser();
            }
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