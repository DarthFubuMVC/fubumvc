using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Security
{
    public class AllowRole : IAuthorizationPolicy
    {
        private readonly string _role;

        public AllowRole(string role)
        {
            _role = role;
        }

        public AuthorizationRight RightsFor(IFubuRequest request)
        {
            return PrincipalRoles.IsInRole(_role) ? AuthorizationRight.Allow : AuthorizationRight.None;
        }
    }
}