using System.Net;
using FubuMVC.Core;
using FubuMVC.Core.Continuations;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Security;
using FubuMVC.Katana;
using FubuMVC.StructureMap;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.Authorization
{
    [TestFixture]
    public class end_to_end_authorization_tester : SharedHarnessContext
    {
        [TearDown]
        public void TearDown()
        {
            SelfHostHarness.Server.Services.Get<SecuritySettings>().Reset();
        }

        [Test]
        public void call_an_endpoint_that_is_not_authorized_and_get_the_403()
        {
            AuthorizationCheck.IsAuthorized = false;

            endpoints.Get<AuthorizedEndpoint>(x => x.get_authorized_text())
                     .StatusCodeShouldBe(HttpStatusCode.Forbidden);
        }

        [Test]
        public void call_an_endpoint_that_is_authorized_with_authorization_disabled()
        {
            AuthorizationCheck.IsAuthorized = false;

            SelfHostHarness.Host.Scenario(_ => {
                _.Security.AuthorizationEnabled = false;
                _.Get.Action<AuthorizedEndpoint>(x => x.get_authorized_text());

                

                _.StatusCodeShouldBeOk();
            });
        }

        [Test]
        public void call_an_endpoint_that_is_authorized_successfully_and_get_The_200_response()
        {
            AuthorizationCheck.IsAuthorized = true;

            endpoints.Get<AuthorizedEndpoint>(x => x.get_authorized_text())
                     .StatusCodeShouldBe(HttpStatusCode.OK);
        }

        [Test]
        public void call_an_endpoint_that_is_not_authorized_with_a_special_auth_failure_handler()
        {
            AuthorizationCheck.IsAuthorized = false;

            endpoints.Get<AuthorizedEndpoint>(x => x.get_authorized_text_special())
                    .StatusCodeShouldBe(HttpStatusCode.Forbidden)
                    .ReadAsText().ShouldEqual("you are forbidden!");
        }


        [Test]
        public void custom_auth_handler()
        {
            var registry = new FubuRegistry();
            registry.Services(x => x.ReplaceService<IAuthorizationFailureHandler, CustomAuthHandler>());

            AuthorizationCheck.IsAuthorized = false;
            using (var server = FubuApplication.For(registry).StructureMap().RunEmbeddedWithAutoPort())
            {
                server.Endpoints.Get<AuthorizedEndpoint>(x => x.get_authorized_text())
                    .StatusCodeShouldBe(HttpStatusCode.Forbidden)
                    .ReadAsText().ShouldEqual("you are forbidden!");
            }
        }

        [Test]
        public void use_custom_auth_handler_on_only_one_endpoint()
        {
            var registry = new FubuRegistry();
            registry.Configure(x => {
                x.BehaviorFor<AuthorizedEndpoint>(_ => _.get_authorized_text()).Authorization.FailureHandler<CustomAuthHandler>();
            });

            AuthorizationCheck.IsAuthorized = false;
            using (var server = FubuApplication.For(registry).StructureMap().RunEmbeddedWithAutoPort())
            {
                server.Endpoints.Get<AuthorizedEndpoint>(x => x.get_authorized_text())
                    .StatusCodeShouldBe(HttpStatusCode.Forbidden)
                    .ReadAsText().ShouldEqual("you are forbidden!");
            }
        }
    }

    public class CustomAuthHandler : IAuthorizationFailureHandler
    {
        public FubuContinuation Handle()
        {
            return FubuContinuation.RedirectTo<AuthFailureEndpoint>(x => x.get_403());
        }
    }



    public class AuthFailureEndpoint
    {
        private readonly IOutputWriter _writer;

        public AuthFailureEndpoint(IOutputWriter writer)
        {
            _writer = writer;
        }

        public string get_403()
        {
            _writer.WriteResponseCode(HttpStatusCode.Forbidden, "No further may you go");
            return "you are forbidden!";
        }
    }

    public static class AuthorizationCheck
    {
        public static bool IsAuthorized;
    }

    public class AuthorizationCheckPolicy : IAuthorizationPolicy
    {
        public AuthorizationRight RightsFor(IFubuRequestContext request)
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

        [AuthorizationFailure(typeof(CustomAuthHandler))]
        public string get_authorized_text_special()
        {
            return "Hola";
        }
    }
}