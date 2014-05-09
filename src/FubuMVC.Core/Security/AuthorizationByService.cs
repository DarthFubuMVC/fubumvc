using System;

namespace FubuMVC.Core.Security
{
    public class AuthorizationByService<T> : IAuthorizationPolicy where T : class
    {
        private readonly Func<T, AuthorizationRight> _func;

        public AuthorizationByService(Func<T, AuthorizationRight> func)
        {
            _func = func;
        }

        public AuthorizationByService(Func<T, bool> filter) : this(x => filter(x) ? AuthorizationRight.Allow : AuthorizationRight.Deny)
        {
        }

        public AuthorizationRight RightsFor(IFubuRequestContext request)
        {
            return _func(request.Service<T>());
        }
    }


}