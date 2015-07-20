using FubuMVC.Core.Continuations;

namespace FubuMVC.Core.Security.Authentication
{
    public class LogoutSuccessHandler : ILogoutSuccessHandler
    {
        public FubuContinuation LoggedOut()
        {
            return FubuContinuation.RedirectTo(new LoginRequest(), "GET");
        }
    }
}