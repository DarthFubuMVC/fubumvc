using FubuMVC.Core.Continuations;

namespace FubuMVC.Authentication
{
    public interface IAuthenticationRedirector
    {
        FubuContinuation Redirect();
    }
}