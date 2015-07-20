namespace FubuMVC.Core.Security.Authentication
{
    public interface IAuthenticationStrategy
    {
        AuthResult TryToApply();
        bool Authenticate(LoginRequest request);
    }
}