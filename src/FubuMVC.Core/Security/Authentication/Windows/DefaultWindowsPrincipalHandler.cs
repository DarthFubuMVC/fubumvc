using System.Security.Principal;

namespace FubuMVC.Core.Security.Authentication.Windows
{
    public class DefaultWindowsPrincipalHandler : IWindowsPrincipalHandler
    {
        public bool Authenticated(IPrincipal principal)
        {
            return true;
        }
    }
}