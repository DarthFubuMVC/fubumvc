using System.Diagnostics;
using FubuMVC.Core;
using FubuMVC.TestingHarness;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuMVC.IntegrationTesting
{
    [TestFixture]
    public class AboutEndpointIntegrationTester : FubuRegistryHarness
    {
        protected override void beforeRunning()
        {
            FubuMode.Mode(FubuMode.Development);
        }

        [Test]
        public void can_get_The_about_page_smoke_test()
        {
            var description = endpoints.Get<AboutEndpoint>(x => x.get__about()).ReadAsText();
            description.ShouldContain("Assemblies");
            Debug.WriteLine(description);
        }
    }
}