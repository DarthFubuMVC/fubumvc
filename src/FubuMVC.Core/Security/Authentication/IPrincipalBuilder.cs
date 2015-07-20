using System.Security.Principal;

namespace FubuMVC.Core.Security.Authentication
{
    public interface IPrincipalBuilder
    {
        IPrincipal Build(string userName);
    }
}