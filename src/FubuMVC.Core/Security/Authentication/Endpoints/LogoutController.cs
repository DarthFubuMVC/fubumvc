using FubuMVC.Core.Continuations;

namespace FubuMVC.Core.Security.Authentication.Endpoints
{
    public class LogoutController
    {
        private readonly IAuthenticationSession _session;
        private readonly ILogoutSuccessHandler _logoutSuccessHandler;

        public LogoutController(IAuthenticationSession session, ILogoutSuccessHandler logoutSuccessHandler)
        {
            _session = session;
            _logoutSuccessHandler = logoutSuccessHandler;
        }

        [UrlPattern("logout")]
        public FubuContinuation Logout(LogoutRequest request)
        {
            _session.ClearAuthentication();
            return _logoutSuccessHandler.LoggedOut();
        }
    }
}