using FubuMVC.Core.Continuations;

namespace FubuMVC.Core.Security.Authentication.Saml2
{
    public interface ISamlDirector
    {
        void SuccessfulUser(string username, FubuContinuation redirection = null);
        void FailedUser(FubuContinuation redirection = null);

        AuthResult Result();
    }
}