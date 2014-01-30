using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FubuMVC.Core.Http;

namespace FubuMVC.OwinHost
{
    using AppFunc = Func<IDictionary<string, object>, Task>;

    public abstract class FubuMvcOwinMiddleware
    {
        private readonly AppFunc _inner;

        protected FubuMvcOwinMiddleware(AppFunc inner)
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