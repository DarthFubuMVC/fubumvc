using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FubuMVC.Core.Http;

namespace FubuMVC.OwinHost.Middleware
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
            var continuation = Invoke(new OwinCurrentHttpRequest(environment), new OwinHttpWriter(environment));

            return continuation.ToTask(environment, _inner);
        }

        public abstract MiddlewareContinuation Invoke(ICurrentHttpRequest request, IHttpWriter writer);
    }
}