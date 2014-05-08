using FubuMVC.Core.Continuations;

namespace FubuMVC.Core.Security
{
    public interface IAuthorizationFailureHandler
    {
        FubuContinuation Handle();
    }
}