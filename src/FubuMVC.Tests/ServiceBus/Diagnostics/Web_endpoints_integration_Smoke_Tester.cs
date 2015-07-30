using System;
using System.Net;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Http.Hosting;
using FubuMVC.Core.ServiceBus;
using FubuMVC.Core.ServiceBus.Diagnostics.Visualization;
using FubuMVC.Core.ServiceBus.InMemory;
using FubuMVC.Katana;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus.Diagnostics
{
    [TestFixture, Explicit("Will bring this back later")]
    public class Web_endpoints_integration_Smoke_Tester
    {
        private readonly string appPath = System.Environment.CurrentDirectory
            .ParentDirectory().ParentDirectory().ParentDirectory()
            .AppendPath("FubuTransportation");


        [Test]
        public void the_message_handlers_visualization_can_be_shown()
        {
            using (var server = EmbeddedFubuMvcServer.For<DiagnosticApplication, KatanaHost>(appPath))
            {
                server.Endpoints.Get<MessagesFubuDiagnostics>(x => x.get_messages())
                    .StatusCode.ShouldBe(HttpStatusCode.OK);
            }

            InMemoryQueueManager.ClearAll();
        }

        [Test]
        public void the_channel_graph_visualization_can_be_shown()
        {
            using (var server = EmbeddedFubuMvcServer.For<DiagnosticApplication, KatanaHost>(appPath))
            {
                server.Endpoints.Get<ChannelGraphFubuDiagnostics>(x => x.get_channels())
                    .StatusCode.ShouldBe(HttpStatusCode.OK);
            }

            InMemoryQueueManager.ClearAll();
        }

        [Test, Explicit("Gets knocked out on CI, think because of pathing")]
        public void the_subscriptions_visualization_can_be_shown()
        {
            using (var server = EmbeddedFubuMvcServer.For<DiagnosticApplication, KatanaHost>(appPath))
            {
                var httpResponse = server.Endpoints.Get<SubscriptionsFubuDiagnostics>(x => x.get_subscriptions());
                if (httpResponse.StatusCode != HttpStatusCode.OK)
                {
                    Assert.Fail(httpResponse.ReadAsText());
                }
            }

            InMemoryQueueManager.ClearAll();
        }

        [Test]
        public void the_scheduled_job_visualization_can_be_shown()
        {
            using (var server = EmbeddedFubuMvcServer.For<DiagnosticApplication, KatanaHost>(appPath))
            {
                server.Endpoints.Get<ScheduledJobsFubuDiagnostics>(x => x.get_scheduled_jobs())
                    .StatusCode.ShouldBe(HttpStatusCode.OK);
            }

            InMemoryQueueManager.ClearAll();
        }
    }

    public class DiagnosticApplication : FubuRegistry, IApplicationSource
    {
        public DiagnosticApplication()
        {
            ServiceBus.EnableInMemoryTransport();
            ServiceBus.Enable(true);

            AlterSettings<TransportSettings>(x =>
            {
                x.DelayMessagePolling = Int32.MaxValue;
                x.ListenerCleanupPolling = Int32.MaxValue;
            });
        }

        public FubuApplication BuildApplication(string directory)
        {
            return FubuApplication.For<DiagnosticApplication>();
        }
    }
}