using System.Net;
using FubuMVC.Core;
using FubuMVC.Core.Ajax;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Security.Authentication;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.IntegrationTesting.Security.Authentication
{
    [TestFixture]
    public class unauthenticated_ajax_request_against_an_authenticated_route : AuthenticationHarness
    {
        [Test]
        public void execute()
        {
            Scenario(_ =>
            {
                _.Get.Input<TargetModel>();
                _.Request.ContentType(MimeType.HttpFormMimetype).Accepts("application/json");

                _.Request.AppendHeader(AjaxExtensions.XRequestedWithHeader, AjaxExtensions.XmlHttpRequestValue);


                _.StatusCodeShouldBe(HttpStatusCode.Unauthorized);

                var continuation = _.Response.Body.ReadAsJson<AjaxContinuation>();

                continuation.Success.ShouldBeFalse();
                var loginUrl = Urls.UrlFor(new LoginRequest
                {
                    Url = null
                }, "GET");

                loginUrl.ShouldEndWith(continuation.NavigatePage);
            }
                );
        }
    }
}