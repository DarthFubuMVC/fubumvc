namespace FubuMVC.Authentication
{
    public interface IAuthenticationStrategy
    {
        AuthResult TryToApply();
        bool Authenticate(LoginRequest request);
    }
}