using System;
using System.Net;
using System.Threading;
using AspNetApplication;
using FubuMVC.Core.Http;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.AspNetTesting
{
    [TestFixture]
    public class reading_cookies
    {
        [Test]
        public void reads_basic_cookie()
        {
            var value = Guid.NewGuid().ToString();

            TestApplication
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
                              Name = "Test1",
                              Value = Guid.NewGuid().ToString()
                          };

            var response = TestApplication.Endpoints.PostAsForm(request);
            response.Cookies[request.Name].Value.ShouldEqual(request.Value);
        }
    }

}