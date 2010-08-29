using System.Net;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Security
{
    public class DefaultAuthorizationFailureHandler : IAuthorizationFailureHandler
    {
        private readonly IOutputWriter _writer;

        public DefaultAuthorizationFailureHandler(IOutputWriter writer)
        {
            _writer = writer;
        }

        public void Handle()
        {
            _writer.WriteResponseCode(HttpStatusCode.Forbidden);
        }
    }
}