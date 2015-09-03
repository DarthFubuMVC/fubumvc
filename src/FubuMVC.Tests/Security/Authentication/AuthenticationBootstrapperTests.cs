using System.Linq;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Security.Authentication;
using Shouldly;
using NUnit.Framework;
using StructureMap;

namespace FubuMVC.Tests.Security.Authentication
{
    [TestFixture]
    public class out_of_the_box_authentication_setup
    {

        [Test]
        public void service_registrations()
        {
            var registry = new FubuRegistry();
            registry.Actions.IncludeType<NothingEndpoint>(); // Have to do this to make it an isolated test
            registry.Features.Authentication.Enable(true);

            using (var runtime = registry.ToRuntime())
            {
                var container = runtime.Get<IContainer>();

                var theGraphWithBasicAuthentication = runtime.Behaviors;

                // This login endpoint was added
                theGraphWithBasicAuthentication.Chains.Where(x => x.InputType() == typeof(LoginRequest))
                    .Count().ShouldBe(2);

                // The logout endpoint was added
                theGraphWithBasicAuthentication.ChainFor(typeof(LogoutRequest)).ShouldNotBeNull();

                container.DefaultRegistrationIs<ILoginSuccessHandler, LoginSuccessHandler>();
                container.DefaultRegistrationIs<ILogoutSuccessHandler, LogoutSuccessHandler>();
            }

        }


    }

    public class NothingEndpoint
    {
    }
}