using FubuMVC.Autofac;
using FubuMVC.Core;
using FubuMVC.Katana;
using FubuMVC.OwinHost;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting
{
    [TestFixture]
    public class Autofac
    {
        [Test]
        public void can_run_an_endpoint_from_end_to_end()
        {
            using (var server = FubuApplication.DefaultPolicies().Autofac().RunEmbedded(port:PortFinder.FindPort(5500)))
            {
                server.Endpoints.Get<AutofacEndpoint>(x => x.get_autofac())
                    .ReadAsText()
                    .ShouldEqual("I'm running on Autofac");
            }
        }
    }

    public class AutofacEndpoint
    {
        public string get_autofac()
        {
            return "I'm running on Autofac";
        }
    }
}