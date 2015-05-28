namespace FubuMVC.Authentication
{
    public interface ICredentialsAuthenticator
    {
        bool AuthenticateCredentials(LoginRequest request);
    }
}