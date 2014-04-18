using FubuMVC.Core.Urls;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.Owin
{
    [TestFixture]
    public class Owin_url_resolution_tester
    {
        [Test]
        public void no_longer_puts_localhost_in_the_resolved_url()
        {
            TestHost.Scenario(_ => {
                _.Get.Action<UrlEndpoints>(x => x.get_green());

                _.ContentShouldBe("/green");
            });
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