using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Security.Authorization
{
    public class AuthorizationCheckPolicy<T> : IAuthorizationPolicy where T : IAuthorizationCheck
    {
        public AuthorizationRight RightsFor(IFubuRequestContext request)
        {
            return request.Service<T>().Check();
        }
    }
}