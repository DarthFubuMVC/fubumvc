using System.ComponentModel;
using System.Security.Principal;
using System.Threading;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Security.Authorization
{
    public class MustBeAuthenticatedAttribute : ModifyChainAttribute
    {
        public override void Alter(ActionCallBase call)
        {
            call.ParentChain().Authorization.AddPolicy(new MustBeAuthenticated());
        }
    }

    /// <summary>
    /// The user must be authenticated
    /// </summary>
    public class MustBeAuthenticated : IAuthorizationPolicy
    {
        public AuthorizationRight RightsFor(IFubuRequestContext request)
        {
            var principal = Thread.CurrentPrincipal;
            return DetermineRights(principal);
        }

        public static AuthorizationRight DetermineRights(IPrincipal principal)
        {
            if (principal == null || principal.Identity == null) return AuthorizationRight.Deny;

            return principal.Identity.IsAuthenticated
                ? AuthorizationRight.Allow
                : AuthorizationRight.Deny;
        }
    }
}