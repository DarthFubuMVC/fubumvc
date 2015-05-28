using FubuMVC.Core.Continuations;

namespace FubuMVC.Authentication
{
    public interface ILogoutSuccessHandler
    {
        FubuContinuation LoggedOut();
    }
}