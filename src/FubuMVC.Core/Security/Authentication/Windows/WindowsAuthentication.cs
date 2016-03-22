using System.Security.Principal;
using FubuMVC.Core.Continuations;
using FubuMVC.Core.Security.Authentication.Auditing;

namespace FubuMVC.Core.Security.Authentication.Windows
{
    public class WindowsAuthentication : IWindowsAuthentication
    {
        private readonly IAuthenticationSession _session;
        private readonly IWindowsPrincipalHandler _handler;
        private readonly ILoginAuditor _auditor;

        public WindowsAuthentication(IAuthenticationSession session, IWindowsPrincipalHandler handler, ILoginAuditor auditor)
        {
            _session = session;
            _handler = handler;
            _auditor = auditor;
        }


        public FubuContinuation Authenticate(WindowsSignInRequest request, WindowsPrincipal principal)
        {
            if (_handler.Authenticated(principal))
            {
                _session.MarkAuthenticated(principal.Identity.Name);
                _auditor.Audit(new SuccessfulWindowsAuthentication{User = principal.Identity.Name});
                
                return FubuContinuation.RedirectTo(request.Url);
            }
            else
            {
                _auditor.Audit(new FailedWindowsAuthentication{User = principal.Identity.Name});

                return FubuContinuation.RedirectTo(new LoginRequest {Url = request.Url}, "GET");
            }

            
            

            
        }
    }
}