using FubuMVC.Core.Urls;
using FubuMVC.Katana;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuMVC.OwinHost.Testing
{
    [TestFixture]
    public class Owin_url_resolution_tester
    {
        [Test]
        public void no_longer_puts_localhost_in_the_resolved_url()
        {
            using (var server = EmbeddedFubuMvcServer.For<HarnessApplication>())
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