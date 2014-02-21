using System;
using FubuMVC.Core;
using FubuMVC.Core.Http;

namespace FubuMVC.OwinHost.Middleware.StaticFiles
{
    public abstract class WriterContinuation : MiddlewareContinuation
    {
        protected WriterContinuation(IHttpResponse response, DoNext doNext)
        {
            if (response == null) throw new ArgumentNullException("response");

            DoNext = doNext;

            Action = () => {
                Write(response);
                response.Flush();
            };
        }

        public abstract void Write(IHttpResponse response);
    }
}