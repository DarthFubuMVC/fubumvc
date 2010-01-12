using System.Security.Principal;

namespace FubuMVC.Core.Security
{
    public interface ISecurityContext
    {
        IIdentity CurrentIdentity { get; }
        IPrincipal CurrentUser { get; set; }
        bool IsAuthenticated();
    }
}