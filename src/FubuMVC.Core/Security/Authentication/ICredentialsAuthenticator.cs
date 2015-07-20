namespace FubuMVC.Core.Security.Authentication
{
    public interface ICredentialsAuthenticator
    {
        bool AuthenticateCredentials(LoginRequest request);
    }
}