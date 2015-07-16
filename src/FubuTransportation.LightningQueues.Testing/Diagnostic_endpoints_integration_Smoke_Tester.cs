using System;
using System.Net;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.StructureMap;
using FubuMVC.Katana;
using FubuTestingSupport;
using FubuTransportation.Configuration;
using FubuTransportation.LightningQueues.Diagnostics;
using NUnit.Framework;

namespace FubuTransportation.LightningQueues.Testing
{
    public class Diagnostic_endpoints_integration_Smoke_Tester
    {
        [Test, Explicit("temporarily broken, Jeremy to fix soon")]
        public void the_lightning_queues_summary_page_can_be_shown()
        {
            // If this test fails on you, try a quick "git clean -xfd" to get rid of the old fubu-content folders,
            // then rake compile to regenerate the bottle content
            using (var server = EmbeddedFubuMvcServer.For<LightningQueuesDiagnosticsApplication>())
            {
                server.Endpoints.Get<LightningQueuesFubuDiagnostics>(x => x.get_queue__managers())
                    .StatusCode.ShouldEqual(HttpStatusCode.OK);
            }
        }
    }

    public class LightningQueuesDiagnosticsApplication : IApplicationSource
    {
        public FubuApplication BuildApplication()
        {
            var registry = new FubuRegistry();
            registry.Import<LightningQueuesDiagnosticsTransportRegistry>();

            return FubuApplication.For(registry).StructureMap();
        }
    }

    public class LightningQueuesDiagnosticsTransportRegistry : FubuTransportRegistry<TransportDiagnosticsSettings>
    {
        public LightningQueuesDiagnosticsTransportRegistry()
        {
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