using System.Security.Principal;

namespace FubuMVC.Core.Security.Authorization
{
    public interface IPrincipalFactory
    {
        IPrincipal CreatePrincipal(IIdentity identity);
    }
}