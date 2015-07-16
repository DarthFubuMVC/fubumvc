using System.Net;
using FubuMVC.Core;
using FubuMVC.Core.StructureMap;
using FubuMVC.Nowin;
using FubuMVC.StructureMap;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting
{
    [TestFixture]
    public class Auto_Nowin_Hosting_Integration_Tester
    {
        [Test]
        public void use_auto_enabled_hosting_from_embedded_server()
        {
            using (var server = FubuApplication.For<NowinRegistry>().RunEmbedded())
            {
                server.Endpoints.Get<NowinEndpoint>(x => x.get_nowin())
                    .ReadAsText()
                    .ShouldEqual("this is served by nowin");
            }
        }

        [Test]
        public void use_auto_enabled_hosting_as_is()
        {
            using (var server = FubuApplication.For<NowinRegistry>().Bootstrap())
            {
                var client = new WebClient();
                client.DownloadString("http://localhost:5601/nowin")
                    .ShouldEqual("this is served by nowin");
            }
        }
    }

    // SAMPLE: nowin-auto-hosting
    public class NowinRegistry : FubuRegistry
    {
        public NowinRegistry()
        {
            Actions.IncludeType<NowinEndpoint>();

            AlterSettings<NowinSettings>(x =>
            {
                // This line is absolutely mandatory
                x.AutoHostingEnabled = true;

                x.Port = 5601;
            });
        }
    }
    // ENDSAMPLE

    public class NowinEndpoint
    {
        public string get_nowin()
        {
            return "this is served by nowin";
        }
    }
}
