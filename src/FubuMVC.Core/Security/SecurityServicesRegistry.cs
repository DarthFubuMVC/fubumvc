using FubuMVC.Core.Registration;
using FubuMVC.Core.Web.Security;

namespace FubuMVC.Core.Security
{
    public class SecurityServicesRegistry : ServiceRegistry
    {
        public SecurityServicesRegistry()
        {
            SetServiceIfNone<IAuthorizationFailureHandler, DefaultAuthorizationFailureHandler>();


            SetServiceIfNone<ISecurityContext, WebSecurityContext>();
            SetServiceIfNone<IAuthenticationContext, WebAuthenticationContext>();

            SetServiceIfNone<IAuthorizationPreviewService, AuthorizationPreviewService>();

            SetServiceIfNone<IAuthorizationPolicyExecutor, AuthorizationPolicyExecutor>();
            SetServiceIfNone<IChainAuthorizor, ChainAuthorizor>();
        }
    }
}