using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Security.Authorization
{
    public class AlwaysDenyPolicy : IAuthorizationPolicy
    {
        public AuthorizationRight RightsFor(IFubuRequestContext request)
        {
            return AuthorizationRight.Deny;
        }
    }
}