using System.Security.Principal;
using System.Web;
using FubuMVC.Core.Security;

namespace FubuMVC.Core.Web.Security
{
    public class WebSecurityContext : ISecurityContext
    {
        private readonly HttpContext _context;

        public WebSecurityContext()
        {
            _context = HttpContext.Current;
        }


        public bool IsAuthenticated()
        {
            return _context.Request.IsAuthenticated;
        }

        public IIdentity CurrentIdentity { get { return _context.User.Identity; } }

        public IPrincipal CurrentUser { get { return _context.User; } set { _context.User = value; } }
    }
}