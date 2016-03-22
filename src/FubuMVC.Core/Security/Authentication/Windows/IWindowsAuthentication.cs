using System.Security.Principal;
using FubuMVC.Core.Continuations;

namespace FubuMVC.Core.Security.Authentication.Windows
{
    public interface IWindowsAuthentication
    {
        FubuContinuation Authenticate(WindowsSignInRequest request, WindowsPrincipal principal);
    }
}