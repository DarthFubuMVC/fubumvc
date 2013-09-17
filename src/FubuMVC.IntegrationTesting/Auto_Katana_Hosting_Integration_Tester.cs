using System.Net;
using FubuMVC.Core;
using FubuMVC.Katana;
using FubuMVC.StructureMap;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.IntegrationTesting
{
    [TestFixture]
    public class Auto_Katana_Hosting_Integration_Tester
    {
        [Test]
        public void use_auto_enabled_hosting_from_embedded_server()
        {
            using (var server = FubuApplication.For<KatanaRegistry>().StructureMap().RunEmbedded())
            {
                server.Endpoints.Get<KatanaEndpoint>(x => x.get_katana())
                    .ReadAsText()
                    .ShouldEqual("this is served by katana");
            }
        }

        [Test]
        public void use_auto_enabled_hosting_as_is()
        {
            using (var runtime = FubuApplication.For<KatanaRegistry>().StructureMap().Bootstrap())
            {
                var client = new WebClient();
                client.DownloadString("http://localhost:5601/katana")
                    .ShouldEqual("this is served by katana");
            }
        }
    }

    public class KatanaRegistry : FubuRegistry
    {
        public KatanaRegistry()
        {
            Actions.IncludeType<KatanaEndpoint>();
            AlterSettings<KatanaSettings>(x => {
                x.Port = 5601;
                x.AutoHostingEnabled = true;
            });
        }
    }

    public class KatanaEndpoint
    {
        public string get_katana()
        {
            return "this is served by katana";
        }
    }
}