using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FubuMVC.Core.Http.Owin.Middleware
{
    public abstract class FubuMvcOwinMiddleware
    {
        private readonly Func<IDictionary<string, object>, Task> _inner;

        protected FubuMvcOwinMiddleware(Func<IDictionary<string, object>, Task> inner)
        {
            _inner = inner;
        }

        public Task Invoke(IDictionary<string, object> environment)
        {
            var continuation = Invoke(new OwinHttpRequest(environment), new OwinHttpResponse(environment));

            return continuation.ToTask(environment, _inner);
        }

        public abstract MiddlewareContinuation Invoke(IHttpRequest request, IHttpResponse response);
    }
}