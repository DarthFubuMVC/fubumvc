using FubuMVC.Core.Continuations;

namespace FubuMVC.Authentication
{
    public interface ILoginSuccessHandler
    {
        FubuContinuation LoggedIn(LoginRequest request);
    }
}