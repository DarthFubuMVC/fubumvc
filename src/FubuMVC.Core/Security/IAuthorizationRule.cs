namespace FubuMVC.Core.Security
{
    public interface IAuthorizationRule<in T>
    {
        AuthorizationRight RightsFor(T model);
    }
}