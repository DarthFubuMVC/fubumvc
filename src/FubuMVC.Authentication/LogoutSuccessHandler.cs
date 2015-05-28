using FubuMVC.Core.Continuations;

namespace FubuMVC.Authentication
{
    public class LogoutSuccessHandler : ILogoutSuccessHandler
    {
        public FubuContinuation LoggedOut()
        {
            return FubuContinuation.RedirectTo(new LoginRequest(), "GET");
        }
    }
}