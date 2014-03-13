using System.Collections.Generic;
using System.Linq;
using System.Net;
using FubuMVC.Core.Http;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting
{
    [TestFixture, Ignore("Test doesn't work because of EndpointDriver and our usage of the underlying HttpWebRequest business")]
    public class read_cookies_from_the_request : SharedHarnessContext
    {
        [Test]
        public void read_a_single_request_cookie()
        {
            endpoints.Get<CookieReceiverEndpoint>(x => x.get_cookies(), configure: r => {
                r.Headers.Add(HttpRequestHeader.Cookie, new Core.Http.Cookies.Cookie("Foo", "Bar").ToString());


            }).ReadAsText().ShouldEqual("Some stuff");
        }

        [Test]
        public void read_multiple_request_cookies()
        {
            endpoints.Get<CookieReceiverEndpoint>(x => x.get_cookies(), configure: r =>
            {
                r.CookieContainer.Add(new Cookie("Foo", "Bar", "Foo", "http://FooDomain.com"));
                r.CookieContainer.Add(new Cookie("Dallas", "Cowboys", "Dallas", "http://DallasDomain.com"));
                
            }).ReadAsText().ShouldEqual("Some stuff");
        }
    }

    public class CookieReceiverEndpoint
    {
        private readonly IHttpRequest _request;

        public CookieReceiverEndpoint(IHttpRequest request)
        {
            _request = request;
        }

        public string get_cookies()
        {
            return _request.Cookies.All.Select(x => x.ToString()).Join("; ");
        }
    }
}