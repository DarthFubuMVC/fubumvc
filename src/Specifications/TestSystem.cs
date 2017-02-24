using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Http.Hosting;
using FubuMVC.Core.Security.Authentication;
using FubuMVC.Core.Security.Authentication.Endpoints;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.Polling;
using Serenity;
using ServiceNode;
using StoryTeller;
using StructureMap;

namespace Specifications
{
    public class WebsiteRegistry : FubuTransportRegistry<TestBusSettings>
    {
        public WebsiteRegistry()
        {
            Channel(x => x.Website).ReadIncoming();
            Channel(x => x.Service).AcceptsMessagesInAssemblyContainingType<ServiceRegistry>();

            ServiceBus.HealthMonitoring.ScheduledExecution(ScheduledExecution.Disabled);


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
        public static void FigureItOut()
        {
            using (var runner = StorytellerRunner.For<TestSystem>())
            {
                runner.Run("Content Negotiation / Default Conneg Rules");
                //runner.RunAll(20.Minutes());
            }
        }

        public TestSystem()
        {
//            AddRemoteSubSystem("ServiceNode", x =>
//            {
//                var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
//                var profile = baseDirectory.Split(Path.PathSeparator).Last();
//
//                x.ServiceDirectory = baseDirectory.ParentDirectory().ParentDirectory().ParentDirectory()
//                    .AppendPath("ServiceNode", "bin", profile);
//
//                x.UseParallelServiceDirectory("ServiceNode");
//                x.Setup.ShadowCopyFiles = false.ToString();
//            });
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

        }
    }
}