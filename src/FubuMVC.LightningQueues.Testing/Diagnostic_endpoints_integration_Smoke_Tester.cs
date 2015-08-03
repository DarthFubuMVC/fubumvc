using System;
using System.Net;
using FubuMVC.Core;
using FubuMVC.Core.Http.Hosting;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.LightningQueues.Diagnostics;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.LightningQueues.Testing
{
    public class Diagnostic_endpoints_integration_Smoke_Tester
    {
        [Test, Explicit("temporarily broken, Jeremy to fix soon")]
        public void the_lightning_queues_summary_page_can_be_shown()
        {
            // If this test fails on you, try a quick "git clean -xfd" to get rid of the old fubu-content folders,
            // then rake compile to regenerate the bottle content
            using (var server = FubuRuntime.For<LightningQueuesDiagnosticsTransportRegistry>())
            {
                server.Endpoints.Get<LightningQueuesFubuDiagnostics>(x => x.get_queue__managers())
                    .StatusCode.ShouldBe(HttpStatusCode.OK);
            }
        }
    }

    public class LightningQueuesDiagnosticsTransportRegistry : FubuTransportRegistry<TransportDiagnosticsSettings>
    {
        public LightningQueuesDiagnosticsTransportRegistry()
        {
            HostWith<Katana>();

            Channel(x => x.Endpoint)
                .ReadIncoming();
        }
    }

    public class TransportDiagnosticsSettings
    {
        public TransportDiagnosticsSettings()
        {
            Endpoint = new Uri("lq.tcp://localhost:2031/diagnostics");
        }

        public Uri Endpoint { get; set; }
    }
}