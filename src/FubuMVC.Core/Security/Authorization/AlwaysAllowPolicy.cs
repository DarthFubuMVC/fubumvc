using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Security.Authorization
{
    public class AlwaysAllowPolicy : IAuthorizationPolicy
    {
        public AuthorizationRight RightsFor(IFubuRequestContext request)
        {
            return AuthorizationRight.Allow;
        }
    }
}