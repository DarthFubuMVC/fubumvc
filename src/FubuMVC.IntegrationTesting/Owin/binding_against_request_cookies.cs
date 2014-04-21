using FubuMVC.Core.Http.Cookies;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.Owin
{
    [TestFixture]
    public class binding_against_request_cookies
    {
        [Test]
        public void can_bind_to_request_cookies()
        {
            var model = new CookieModel
            {
                Color = "Green",
                Direction = "South"
            };

            TestHost.Scenario(_ => {
                _.Get.Input(model);

                _.Request.AppendCookie(new Cookie("Color", "Green"));
                _.Request.AppendCookie(new Cookie("Direction", "South"));

                _.ContentShouldBe(model.ToString());
            });
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