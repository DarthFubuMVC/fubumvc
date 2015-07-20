using FubuMVC.Core.Continuations;

namespace FubuMVC.Core.Security.Authorization
{
    public interface IAuthorizationFailureHandler
    {
        FubuContinuation Handle();
    }
}