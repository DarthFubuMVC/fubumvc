using FubuMVC.Core.Continuations;

namespace FubuMVC.Core.Security.Authentication
{
    public interface ILoginSuccessHandler
    {
        FubuContinuation LoggedIn(LoginRequest request);
    }
}