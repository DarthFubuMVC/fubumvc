using System.Net;
using AspNetApplication;
using FubuMVC.AspNetTesting;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.SelfHost.Testing
{
    [TestFixture]
    public class binding_against_request_cookies
    {
        [Test]
        public void can_bind_to_request_cookies()
        {
            var model = new CookieModel{
                Color = "Green",
                Direction = "South"
            };

            TestApplication.Endpoints.GetByInput(model, configure: SetupCookies).ReadAsText()
                .ShouldEqual(model.ToString());
        }

        public void SetupCookies(HttpWebRequest request)
        {
            //request.Headers[HttpRequestHeader.Cookie] = "Color=Green;Direction=South";
            request.CookieContainer.Add(new Cookie("Color", "Green", "/", "localhost"));
            request.CookieContainer.Add(new Cookie("Direction", "South", "/", "localhost"));
        }
    }
}