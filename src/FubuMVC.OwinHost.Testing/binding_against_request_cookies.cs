using System.Net;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.OwinHost.Testing
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

            Harness.Endpoints.GetByInput(model, configure: SetupCookies).ReadAsText()
                .ShouldEqual(model.ToString());
        }

        public void SetupCookies(HttpWebRequest request)
        {
            //request.Headers[HttpRequestHeader.Cookie] = "Color=Green;Direction=South";
            request.CookieContainer.Add(new Cookie("Color", "Green", "/", "localhost"));
            request.CookieContainer.Add(new Cookie("Direction", "South", "/", "localhost"));
        }
    }

    public class CookieBindingEndpoint
    {
        public string get_cookie_data(CookieModel input)
        {
            return input.ToString();
        }
    }

    public class CookieModel
    {
        public string Color { get; set; }
        public string Direction { get; set; }

        public override string ToString()
        {
            return string.Format("Color: {0}, Direction: {1}", Color, Direction);
        }
    }
}