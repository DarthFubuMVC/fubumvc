using System.Security.Principal;

namespace FubuMVC.Core.Security.Authentication.Windows
{
    public interface IWindowsPrincipalHandler
    {
        bool Authenticated(IPrincipal principal);
    }
}