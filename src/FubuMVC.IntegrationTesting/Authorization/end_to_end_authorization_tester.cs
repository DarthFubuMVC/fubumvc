using System.Net;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Security;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.Authorization
{
    [TestFixture]
    public class end_to_end_authorization_tester : SharedHarnessContext
    {
        [Test]
        public void call_an_endpoint_that_is_not_authorized_and_get_the_403()
        {
            AuthorizationCheck.IsAuthorized = false;

            endpoints.Get<AuthorizedEndpoint>(x => x.get_authorized_text())
                     .StatusCodeShouldBe(HttpStatusCode.Forbidden);
        }

        [Test]
        public void call_an_endpoint_that_is_authorized_successfully_and_get_The_200_response()
        {
            AuthorizationCheck.IsAuthorized = true;

            endpoints.Get<AuthorizedEndpoint>(x => x.get_authorized_text())
                     .StatusCodeShouldBe(HttpStatusCode.OK);
        }
    }

    public static class AuthorizationCheck
    {
        public static bool IsAuthorized;
    }

    public class AuthorizationCheckPolicy : IAuthorizationPolicy
    {
        public AuthorizationRight RightsFor(IFubuRequest request)
        {
            return AuthorizationCheck.IsAuthorized ? AuthorizationRight.Allow : AuthorizationRight.Deny;
        }
    }

    [AuthorizedBy(typeof(AuthorizationCheckPolicy))]
    public class AuthorizedEndpoint
    {
        public string get_authorized_text()
        {
            return "Hello";
        }
    }
}