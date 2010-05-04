using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Web;

namespace FubuMVC.UI
{
    public static class PrincipalRoles
    {
        public static IPrincipal Current
        {
            get
            {
                return HttpContext.Current == null ? Thread.CurrentPrincipal : HttpContext.Current.User;
            }
        }

        public static bool IsInRole(params string[] roles)
        {
            var principal = Current;
            return roles.Any(r => principal.IsInRole(r));
        }
    }
}