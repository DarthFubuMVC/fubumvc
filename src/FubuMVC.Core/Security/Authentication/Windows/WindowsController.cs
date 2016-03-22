using FubuMVC.Core.Continuations;
using FubuMVC.Core.Security.Authorization;

namespace FubuMVC.Core.Security.Authentication.Windows
{
    [NotAuthenticated]
    public class WindowsController
    {
        private readonly IWindowsAuthenticationContext _context;
        private readonly IWindowsAuthentication _strategy;

        public WindowsController(IWindowsAuthenticationContext context, IWindowsAuthentication strategy)
        {
            _context = context;
            _strategy = strategy;
        }

        [UrlPattern("login/windows")]
        public FubuContinuation Login(WindowsSignInRequest request)
        {
            var principal = _context.Current();
            return _strategy.Authenticate(request, principal);
        }
    }
}