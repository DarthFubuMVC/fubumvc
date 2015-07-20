using System.Collections.Generic;

namespace FubuMVC.Core.Security.Authentication
{
    public interface IAuthenticationService
    {
        AuthResult TryToApply();
        bool Authenticate(LoginRequest request);
        bool Authenticate(LoginRequest request, IEnumerable<IAuthenticationStrategy> strategies);
        void SetRememberMeCookie(LoginRequest request);
    }
}