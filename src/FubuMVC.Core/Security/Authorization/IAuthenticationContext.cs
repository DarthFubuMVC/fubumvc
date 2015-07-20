namespace FubuMVC.Core.Security.Authorization
{
    public interface IAuthenticationContext
    {
        void ThisUserHasBeenAuthenticated(string username, bool rememberMe);
        void SignOut();
    }
}