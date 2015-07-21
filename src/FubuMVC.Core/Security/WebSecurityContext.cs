using System.Security.Principal;
using System.Web;
using FubuMVC.Core.Security.Authorization;

namespace FubuMVC.Core.Security
{
    public class WebSecurityContext : ISecurityContext
    {
        private readonly HttpContextBase _context;

        public WebSecurityContext(HttpContextBase httpContext)
        {
            _context = httpContext;
        }


        public bool IsAuthenticated()
        {
            return _context.Request.IsAuthenticated;
        }

        public IIdentity CurrentIdentity { get { return _context.User.Identity; } }

        public IPrincipal CurrentUser { get { return _context.User; } set { _context.User = value; } }
    }
}