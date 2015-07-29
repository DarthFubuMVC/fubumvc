using System.Diagnostics;
using System.Threading;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Http.Hosting;
using FubuMVC.Core.Runtime;
using FubuMVC.Katana;
using HtmlTags;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting
{
    [TestFixture]
    public class AboutEndpointIntegrationTester
    {
        private EmbeddedFubuMvcServer server;

        [TestFixtureSetUp]
        public void SetUp()
        {
            FubuMode.Mode(FubuMode.Development);

            server = FubuApplication.DefaultPolicies().RunEmbedded(port: PortFinder.FindPort(5500));
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            FubuMode.Reset();
            server.SafeDispose();
        }

        [Test]
        public void can_get_The_about_page_smoke_test()
        {
            TestHost.Scenario(_ =>
            {
                _.Get.Action<AboutFubuDiagnostics>(x => x.get_about());
                _.ContentShouldContain("Assemblies");
            });
        }


        [Test, Explicit]
        public void manual_test_the_auto_reloading_tag()
        {
            FubuMode.Mode(FubuMode.Development);

            using (var server = FubuApplication.DefaultPolicies().RunEmbedded(port: 5601))
            {
                Process.Start("http://localhost:5601/reloaded");
                Thread.Sleep(20000);
            }

            using (var server = FubuApplication.DefaultPolicies().RunEmbedded(port: 5601))
            {
                Thread.Sleep(20000);
            }

            FubuMode.Reset();
        }
    }


    public class ReloadingEndpoint
    {
        private readonly AppReloaded _reloaded;

        public ReloadingEndpoint(AppReloaded reloaded)
        {
            _reloaded = reloaded;
        }

        public HtmlDocument get_reloaded()
        {
            var document = new HtmlDocument();
            document.Title = "Manual Test Harness for reloading";
            document.Add("h1").Text("Loaded at " + _reloaded.Timestamp);

            document.Add(new AutoReloadingTag());

            return document;
        }
    }
}