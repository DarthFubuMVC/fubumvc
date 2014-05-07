namespace FubuMVC.Core.Security
{
    public interface IAuthorizationPolicy
    {
        AuthorizationRight RightsFor(IFubuRequestContext request);
    }
}