namespace FubuMVC.Core.Security
{
    public class AuthorizationCheckPolicy<T> : IAuthorizationPolicy where T : IAuthorizationCheck
    {
        public AuthorizationRight RightsFor(IFubuRequestContext request)
        {
            return request.Service<T>().Check();
        }
    }
}