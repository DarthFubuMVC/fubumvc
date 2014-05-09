namespace FubuMVC.Core.Security
{
    public class AlwaysAllowPolicy : IAuthorizationPolicy
    {
        public AuthorizationRight RightsFor(IFubuRequestContext request)
        {
            return AuthorizationRight.Allow;
        }
    }
}