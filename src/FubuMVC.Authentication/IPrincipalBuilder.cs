using System.Security.Principal;

namespace FubuMVC.Authentication
{
    public interface IPrincipalBuilder
    {
        IPrincipal Build(string userName);
    }
}