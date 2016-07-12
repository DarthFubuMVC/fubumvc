using System;
using System.Threading.Tasks;

namespace FubuMVC.Core.Http.Owin.Middleware.StaticFiles
{
    public abstract class WriterContinuation : MiddlewareContinuation
    {
        protected WriterContinuation(IHttpResponse response, DoNext doNext)
        {
            if (response == null) throw new ArgumentNullException(nameof(response));

            DoNext = doNext;

            Action = async () => {
                await Write(response).ConfigureAwait(false);
                response.Flush();
            };
        }

        public abstract Task Write(IHttpResponse response);
    }
}