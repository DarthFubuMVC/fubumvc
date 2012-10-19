using System.Linq;
using System.Net;
using FubuMVC.Core.Http;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.SelfHost.Testing
{
    [TestFixture]
    public class reading_cookies
    {
        [Test]
        public void reads_basic_cookie()
        {
            SelfHostHarness
                .Endpoints
                .GetByInput(new CookieInfo(), "GET", configure: request =>
                {
                    var cookie = new Cookie("Test", "Test", "/", "localhost");

                    var cookies = new CookieContainer();
                    cookies.Add(cookie);

                    request.CookieContainer = cookies;
                });
        }
    }

    public class CookieEndpoint
    {
        private readonly ICookies _cookies;

        public CookieEndpoint(ICookies cookies)
        {
            _cookies = cookies;
        }

        public string get_cookie_info(CookieInfo info)
        {
            _cookies.Request.ToList();
            return string.Empty;
        }
    }
    public class CookieInfo { }
}