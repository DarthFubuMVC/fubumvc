using FubuMVC.Core;
using FubuMVC.Core.Urls;
using FubuMVC.Katana;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.Owin
{
    [TestFixture]
    public class Owin_url_resolution_tester
    {
        [Test]
        public void no_longer_puts_localhost_in_the_resolved_url()
        {
            using (var server = EmbeddedFubuMvcServer.For<HarnessApplication>(port:PortFinder.FindPort(5510)))
            {
                server.Endpoints.Get<UrlEndpoints>(x => x.get_green())
                   .ReadAsText().ShouldEqual("/green");
            }
        }
    }

    public class UrlEndpoints
    {
        private readonly IUrlRegistry _urls;

        public UrlEndpoints(IUrlRegistry urls)
        {
            _urls = urls;
        }
        
        public string get_green()
        {
            return _urls.UrlFor<UrlEndpoints>(x => x.get_green());
        }
    }
}