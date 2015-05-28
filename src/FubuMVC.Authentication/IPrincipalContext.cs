using System.Security.Principal;

namespace FubuMVC.Authentication
{
    public interface IPrincipalContext
    {
        IPrincipal Current
        {
            get; set;
        }
    }
}