using System.Security.Principal;

namespace FubuMVC.Core.Security.Authorization
{
    public interface ISecurityContext
    {
        IIdentity CurrentIdentity { get; }
        IPrincipal CurrentUser { get; set; }
        bool IsAuthenticated();
    }
}