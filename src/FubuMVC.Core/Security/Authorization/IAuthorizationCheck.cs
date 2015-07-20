namespace FubuMVC.Core.Security.Authorization
{
    public interface IAuthorizationCheck
    {
        AuthorizationRight Check();
    }
}