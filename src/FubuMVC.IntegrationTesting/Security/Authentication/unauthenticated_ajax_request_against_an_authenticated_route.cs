using System.Net;
using FubuMVC.Core;
using FubuMVC.Core.Ajax;
using FubuMVC.Core.Endpoints;
using FubuMVC.Core.Security.Authentication;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.Security.Authentication
{
    [TestFixture]
    public class unauthenticated_ajax_request_against_an_authenticated_route : AuthenticationHarness
    {
        private HttpResponse theResponse;

        protected override void beforeEach()
        {
            theResponse = endpoints.GetByInput(new TargetModel(), acceptType: "application/json", configure: r =>
            {
                r.AllowAutoRedirect = false;
                r.Headers.Add(AjaxExtensions.XRequestedWithHeader, AjaxExtensions.XmlHttpRequestValue);
            });
        }

        [Test]
        public void unauthorized_status_code()
        {
            theResponse.StatusCode.ShouldEqual(HttpStatusCode.Unauthorized);
        }

        [Test]
        public void writes_the_navigate_continuation()
        {
            var continuation = theResponse.ReadAsJson<AjaxContinuation>();

            continuation.Success.ShouldBeFalse();
            var loginUrl = Urls.UrlFor(new LoginRequest
            {
                Url = null
            }, "GET");

            loginUrl.ShouldEndWith(continuation.NavigatePage);
        }
    }
}