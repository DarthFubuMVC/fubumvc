using System.Diagnostics;
using System.Threading;
using FubuMVC.Core;
using FubuMVC.Core.Diagnostics;
using FubuMVC.OwinHost;
using FubuMVC.TestingHarness;
using NUnit.Framework;
using FubuTestingSupport;
using FubuMVC.StructureMap;
using StructureMap;
using FubuMVC.Katana;

namespace FubuMVC.IntegrationTesting
{
    [TestFixture]
    public class AboutEndpointIntegrationTester
    {
        [SetUp]
        public void SetUp()
        {
            FubuMode.Mode(FubuMode.Development);
        }

        [TearDown]
        public void TearDown()
        {
            FubuMode.Reset();
        }

        [Test]
        public void can_get_The_about_page_smoke_test()
        {
            using (var server = FubuApplication.DefaultPolicies().StructureMap().RunEmbedded(port:PortFinder.FindPort(5500)))
            {
                var description = server.Endpoints.Get<AboutEndpoint>(x => x.get__about()).ReadAsText();
                description.ShouldContain("Assemblies");
                Debug.WriteLine(description);
            }
        }

        [Test]
        public void can_get_the_reloaded_time_consistently()
        {
            string ts1;
            string ts2;
            string ts3;
            string ts4;

            using (var server = FubuApplication.DefaultPolicies().StructureMap().RunEmbedded(port: PortFinder.FindPort(5500)))
            {
                ts1 = server.Endpoints.Get<AboutEndpoint>(x => x.get__loaded()).ReadAsText();
                ts2 = server.Endpoints.Get<AboutEndpoint>(x => x.get__loaded()).ReadAsText();
            }

            Thread.Sleep(100);

            using (var server = FubuApplication.DefaultPolicies().StructureMap().RunEmbedded(port: PortFinder.FindPort(5500)))
            {
                ts3 = server.Endpoints.Get<AboutEndpoint>(x => x.get__loaded()).ReadAsText();
                ts4 = server.Endpoints.Get<AboutEndpoint>(x => x.get__loaded()).ReadAsText();
            }

            ts1.ShouldEqual(ts2);

            ts3.ShouldEqual(ts4);

            ts1.ShouldNotEqual(ts3);
        }
    }
}