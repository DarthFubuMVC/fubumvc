using FubuMVC.Core.Continuations;

namespace FubuMVC.Core.Security.Authentication
{
    public interface ILogoutSuccessHandler
    {
        FubuContinuation LoggedOut();
    }
}