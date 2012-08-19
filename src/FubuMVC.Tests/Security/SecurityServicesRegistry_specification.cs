using FubuMVC.Core.Registration;
using FubuMVC.Core.Security;
using FubuMVC.Core.Web.Security;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Security
{
    [TestFixture]
    public class SecurityServicesRegistry_specification
    {
        private void registeredTypeIs<TService, TImplementation>()
        {
            BehaviorGraph.BuildEmptyGraph().Services.DefaultServiceFor<TService>().Type.ShouldEqual(
                typeof(TImplementation));
        }

        [Test]
        public void IAuthenticationContext_is_registered()
        {
            registeredTypeIs<IAuthenticationContext, WebAuthenticationContext>();
        }

        [Test]
        public void ISecurityContext_is_registered()
        {
            registeredTypeIs<ISecurityContext, WebSecurityContext>();
        }

        [Test]
        public void authorization_preview_service_is_registered()
        {
            registeredTypeIs<IAuthorizationPreviewService, AuthorizationPreviewService>();
        }

        [Test]
        public void chain_authorizor_is_registered()
        {
            registeredTypeIs<IChainAuthorizor, ChainAuthorizor>();
        }


        [Test]
        public void default_authorization_failure_handler_is_registered()
        {
            registeredTypeIs<IAuthorizationFailureHandler, DefaultAuthorizationFailureHandler>();
        }
    }
}