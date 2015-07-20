using System.Security.Principal;
using System.Threading;
using System.Web;

namespace FubuMVC.Core.Security.Authentication
{
    public class ThreadPrincipalContext : IPrincipalContext
    {
        public IPrincipal Current
        {
            get { return Thread.CurrentPrincipal; }
            set
            {
                Thread.CurrentPrincipal = value;
                if (HttpContext.Current != null)
                {
                    HttpContext.Current.User = value;
                }
            }
        }
    }
}