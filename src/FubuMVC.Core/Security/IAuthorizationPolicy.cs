using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Security
{
    public interface IAuthorizationPolicy
    {
        AuthorizationRight RightsFor(IFubuRequest request); 
    }
}