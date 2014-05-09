namespace FubuMVC.Core.Security
{
    public interface IAuthorizationCheck
    {
        AuthorizationRight Check();
    }
}