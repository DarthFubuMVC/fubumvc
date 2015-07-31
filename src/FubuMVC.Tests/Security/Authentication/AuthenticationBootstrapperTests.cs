using System.Linq;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Security.Authentication;
using Shouldly;
using NUnit.Framework;

namespace FubuMVC.Tests.Security.Authentication
{
    [TestFixture]
    public class out_of_the_box_authentication_setup
    {
        private BehaviorGraph theGraphWithBasicAuthentication;

        [Test]
        public void service_registrations()
        {
            var registry = new FubuRegistry();
            registry.Actions.IncludeType<NothingEndpoint>(); // Have to do this to make it an isolated test
            registry.Features.Authentication.Enable(true);

            using (var runtime = registry.ToRuntime())
            {
                var theGraphWithBasicAuthentication = runtime.Behaviors;

                // This login endpoint was added
                theGraphWithBasicAuthentication.Behaviors.Where(x => x.InputType() == typeof(LoginRequest))
                    .Count().ShouldBe(2);

                // The logout endpoint was added
                theGraphWithBasicAuthentication.BehaviorFor(typeof(LogoutRequest)).ShouldNotBeNull();

                runtime.Container.DefaultRegistrationIs<ILoginSuccessHandler, LoginSuccessHandler>();
                runtime.Container.DefaultRegistrationIs<ILogoutSuccessHandler, LogoutSuccessHandler>();
            }

        }


    }

    public class NothingEndpoint
    {
    }
}