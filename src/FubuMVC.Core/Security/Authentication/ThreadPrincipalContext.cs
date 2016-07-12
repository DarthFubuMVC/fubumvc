using System.Security.Principal;
using System.Threading;
using System.Web;

namespace FubuMVC.Core.Security.Authentication
{
    public class ThreadPrincipalContext : IPrincipalContext
    {
        private IPrincipal _principal;

        public IPrincipal Current
        {
            get { return _principal; }
            set
            {
                _principal = value;
                Thread.CurrentPrincipal = value;
                if (HttpContext.Current != null)
                {
                    HttpContext.Current.User = value;
                }
            }
        }
    }
}