namespace FubuMVC.Core.Security
{
    public interface IAuthenticationContext
    {
        void ThisUserHasBeenAuthenticated(string username, bool rememberMe);
        void SignOut();
    }
}