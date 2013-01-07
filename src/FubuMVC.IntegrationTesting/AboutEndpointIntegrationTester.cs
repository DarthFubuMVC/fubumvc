using System.Diagnostics;
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
            using (var server = FubuApplication.DefaultPolicies().StructureMap(new Container()).RunEmbedded(port:PortFinder.FindPort(5500)))
            {
                var description = server.Endpoints.Get<AboutEndpoint>(x => x.get__about()).ReadAsText();
                description.ShouldContain("Assemblies");
                Debug.WriteLine(description);
            }
        }
    }
}