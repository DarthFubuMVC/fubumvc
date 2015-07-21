using FubuMVC.Core.Registration;

namespace FubuMVC.Core.Security.Authorization
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