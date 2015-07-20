using System.Linq;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Security.Authentication;
using FubuMVC.Core.Security.Authentication.Endpoints;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Security.Authentication
{
    [TestFixture]
    public class out_of_the_box_authentication_setup
    {
        private BehaviorGraph theGraphWithBasicAuthentication;

        [SetUp]
        public void SetUp()
        {
            var registry = new FubuRegistry();
            registry.Actions.IncludeType<NothingEndpoint>(); // Have to do this to make it an isolated test
            registry.Import<ApplyAuthentication>();
            registry.Import<FormsAuthenticationRegistry>();

            theGraphWithBasicAuthentication = BehaviorGraph.BuildFrom(registry);
        }

        [Test]
        public void login_endpoint_is_added()
        {
            theGraphWithBasicAuthentication.Behaviors.Where(x => x.InputType() == typeof (LoginRequest))
                .Count().ShouldEqual(2);
        }

        [Test]
        public void logout_endpoint_is_added()
        {
            theGraphWithBasicAuthentication.BehaviorFor(typeof (LogoutRequest)).ShouldNotBeNull();
        }


        [Test]
        public void basic_login_success_handler_is_registered()
        {
            theGraphWithBasicAuthentication.Services.DefaultServiceFor<ILoginSuccessHandler>()
                .Type.ShouldEqual(typeof (LoginSuccessHandler));
        }

        [Test]
        public void basic_logout_success_handler_is_registered()
        {
            theGraphWithBasicAuthentication.Services.DefaultServiceFor<ILogoutSuccessHandler>()
                .Type.ShouldEqual(typeof (LogoutSuccessHandler));
        }
    }

    public class NothingEndpoint{}
}