using System;
using System.Security.Principal;
using System.Web;

namespace FubuMVC.Core.Security.Authentication.Windows
{
    public class AspNetWindowsAuthenticationContext : IWindowsAuthenticationContext
    {
        private readonly HttpContextBase _context;

        public AspNetWindowsAuthenticationContext(HttpContextBase context)
        {
            _context = context;
        }

        public WindowsPrincipal Current()
        {
            var identity = _context.User?.Identity as WindowsIdentity;
            if (identity == null)
            {
                throw new InvalidOperationException("User identity must be a WindowsIdentity");
            }

            return identity == null ? null : new WindowsPrincipal(identity);
        }
    }
}
