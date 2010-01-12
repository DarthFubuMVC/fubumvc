using System.Security.Principal;

namespace FubuMVC.Core.Security
{
    public interface IPrincipalFactory
    {
        IPrincipal CreatePrincipal(IIdentity identity);
    }
}