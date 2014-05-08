using System.Net;
using FubuMVC.Core.Continuations;

namespace FubuMVC.Core.Security
{
    public class DefaultAuthorizationFailureHandler : IAuthorizationFailureHandler
    {
        public FubuContinuation Handle()
        {
            return FubuContinuation.EndWithStatusCode(HttpStatusCode.Forbidden);
        }
    }
}