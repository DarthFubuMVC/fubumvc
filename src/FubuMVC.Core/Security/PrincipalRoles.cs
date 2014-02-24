using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Web;

namespace FubuMVC.Core.Security
{
    public static class PrincipalRoles
    {
        private static IPrincipal _stubbedPrincipal;

        public static IPrincipal Current
        {
            get
            {
                return _stubbedPrincipal ?? (System.Web.HttpContext.Current == null ? Thread.CurrentPrincipal : System.Web.HttpContext.Current.User);
            }

            set { _stubbedPrincipal = value; }
        }

        public static bool IsInRole(params string[] roles)
        {
            var principal = Current;
            return roles.Any(r => principal.IsInRole(r));
        }

        public static void SetCurrentRolesForTesting(params string[] roles)
        {
            var principal = new GenericPrincipal(new GenericIdentity("somebody"), roles);
            Thread.CurrentPrincipal = principal;
        }
    }
}