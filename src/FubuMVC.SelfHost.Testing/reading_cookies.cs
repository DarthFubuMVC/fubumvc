using System;
using System.Linq;
using System.Net;
using System.Web;
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

    [TestFixture]
    public class writing_cookies
    {
        [Test]
        public void writes_basic_cookies()
        {
            var request = new WriteCookieRequest
                          {
                              Name = "Test",
                              Value = Guid.NewGuid().ToString()
                          };

            var response = SelfHostHarness.Endpoints.PostAsForm(request);
            response.Cookies[request.Name].Value.ShouldEqual(request.Value);
        }
    }

    public class CookieEndpoint
    {
        public const string CookieName = "Test";

        private readonly ICookies _cookies;
        private readonly IHttpWriter _writer;

        public CookieEndpoint(ICookies cookies, IHttpWriter writer)
        {
            _cookies = cookies;
            _writer = writer;
        }

        public string get_cookie_info(CookieInfo info)
        {
            return _cookies.Get(CookieName).Value;
        }

        public void post_write_cookie(WriteCookieRequest request)
        {
            _writer.AppendCookie(new HttpCookie(request.Name, request.Value));
        }
    }

    public class CookieInfo { }
    public class WriteCookieRequest
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}