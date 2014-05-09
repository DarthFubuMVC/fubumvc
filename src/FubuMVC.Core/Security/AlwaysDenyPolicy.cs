namespace FubuMVC.Core.Security
{
    public class AlwaysDenyPolicy : IAuthorizationPolicy
    {
        public AuthorizationRight RightsFor(IFubuRequestContext request)
        {
            return AuthorizationRight.Deny;
        }
    }
}