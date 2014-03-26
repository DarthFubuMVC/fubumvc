using System.Net;
using System.Threading;
using FubuMVC.Core;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Katana;
using FubuMVC.StructureMap;
using FubuTestingSupport;
using NUnit.Framework;
using Process = System.Diagnostics.Process;

namespace FubuMVC.Diagnostics.Tests
{
    [TestFixture, Explicit]
    public class IntegrationTestHarness
    {
        [Test]
        public void run_in_verbose_mode()
        {
            using (var server = FubuApplication.DefaultPolicies().StructureMap().RunEmbedded(@"c:\code\FubuMVC.Diagnostics\src\FubuMVC.Diagnostics"))
            {
                server.Endpoints.PostAsForm(new Input {Name = "Jeremy", Age = 39, Direction = "North"})
                    .StatusCode.ShouldEqual(HttpStatusCode.OK);

                Process.Start(server.BaseAddress + "/_fubu");

                Thread.Sleep(60000);
            }
        }

        [Test]
        public void run_in_production_mode()
        {
            var registry = new FubuRegistry();
            registry.AlterSettings<DiagnosticsSettings>(x => x.TraceLevel = TraceLevel.Production);

            using (var server = FubuApplication.For(registry).StructureMap().RunEmbedded(@"c:\code\FubuMVC.Diagnostics\src\FubuMVC.Diagnostics"))
            {
                server.Endpoints.PostAsForm(new Input { Name = "Jeremy", Age = 39, Direction = "North" })
                    .StatusCode.ShouldEqual(HttpStatusCode.OK);

                Process.Start(server.BaseAddress + "/_fubu");

                Thread.Sleep(60000);
            }
        }

        [Test]
        public void run_in_none_mode()
        {
            var registry = new FubuRegistry();
            registry.AlterSettings<DiagnosticsSettings>(x => x.TraceLevel = TraceLevel.None);

            using (var server = FubuApplication.For(registry).StructureMap().RunEmbedded(@"c:\code\FubuMVC.Diagnostics\src\FubuMVC.Diagnostics"))
            {
                server.Endpoints.PostAsForm(new Input { Name = "Jeremy", Age = 39, Direction = "North" })
                    .StatusCode.ShouldEqual(HttpStatusCode.OK);

                Process.Start(server.BaseAddress + "/_fubu");

                Thread.Sleep(60000);
            }
        }
    }


    public class IntegrationEndpoint
    {
        public string get_text(Input input)
        {
            return input.ToString();
        }

        public string post_text(Input input)
        {
            return input.ToString();
        }        
    }

    public class Input
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public string Direction { get; set; }

        public override string ToString()
        {
            return string.Format("Name: {0}, Age: {1}, Direction: {2}", Name, Age, Direction);
        }
    }
}