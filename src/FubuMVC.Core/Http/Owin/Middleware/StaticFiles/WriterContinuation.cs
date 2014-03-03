using System;

namespace FubuMVC.Core.Http.Owin.Middleware.StaticFiles
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