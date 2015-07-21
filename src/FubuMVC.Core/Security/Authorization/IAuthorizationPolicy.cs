using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Security.Authorization
{
    public interface IAuthorizationPolicy
    {
        AuthorizationRight RightsFor(IFubuRequestContext request);
    }
}