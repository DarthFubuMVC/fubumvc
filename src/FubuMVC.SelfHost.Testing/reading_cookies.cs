using System;
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
            var value = Guid.NewGuid().ToString();

            SelfHostHarness
                .Endpoints
                .GetByInput(new CookieInfo(), "GET", configure: request =>
                {
                    var cookie = new Cookie(CookieEndpoint.CookieName, value, "/", "localhost");

                    var cookies = new CookieContainer();
                    cookies.Add(cookie);

                    request.CookieContainer = cookies;
                }).ReadAsText().ShouldEqual(value);
        }
    }

    public class CookieEndpoint
    {
        public const string CookieName = "Test";

        private readonly ICookies _cookies;

        public CookieEndpoint(ICookies cookies)
        {
            _cookies = cookies;
        }

        public string get_cookie_info(CookieInfo info)
        {
            return _cookies.Get(CookieName).Value;
        }
    }
    public class CookieInfo { }
}