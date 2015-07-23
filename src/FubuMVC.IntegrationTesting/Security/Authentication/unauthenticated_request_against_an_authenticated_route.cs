using System.Net;
using FubuMVC.Core.Security.Authentication;
using Shouldly;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.Security.Authentication
{
    [TestFixture]
    public class unauthenticated_request_against_an_authenticated_route : AuthenticationHarness
    {
        [Test]
        public void redirects_to_login()
        {
            var response = endpoints.GetByInput(new TargetModel(), acceptType: "text/html", configure: r => r.AllowAutoRedirect = false);
            response.StatusCode.ShouldBe(HttpStatusCode.Redirect);

            var loginUrl = Urls.UrlFor(new LoginRequest { Url = "some/authenticated/route"}, "GET");
            loginUrl.ShouldEndWith(response.ResponseHeaderFor(HttpResponseHeader.Location).TrimStart('/'));
        }
    }
}