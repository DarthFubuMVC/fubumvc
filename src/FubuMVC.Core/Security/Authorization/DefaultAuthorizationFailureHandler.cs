using System.Net;
using FubuMVC.Core.Continuations;

namespace FubuMVC.Core.Security.Authorization
{
    public class DefaultAuthorizationFailureHandler : IAuthorizationFailureHandler
    {
        public FubuContinuation Handle()
        {
            return FubuContinuation.EndWithStatusCode(HttpStatusCode.Forbidden);
        }
    }
}