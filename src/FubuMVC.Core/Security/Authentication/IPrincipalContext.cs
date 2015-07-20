using System.Security.Principal;

namespace FubuMVC.Core.Security.Authentication
{
    public interface IPrincipalContext
    {
        IPrincipal Current
        {
            get; set;
        }
    }
}