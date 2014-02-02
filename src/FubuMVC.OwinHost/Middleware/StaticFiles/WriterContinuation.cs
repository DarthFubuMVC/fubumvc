using System;
using FubuMVC.Core;
using FubuMVC.Core.Http;

namespace FubuMVC.OwinHost.Middleware.StaticFiles
{
    public abstract class WriterContinuation : MiddlewareContinuation
    {
        private readonly DoNext _doNext;

        protected WriterContinuation(IHttpWriter writer, DoNext doNext)
        {
            if (writer == null) throw new ArgumentNullException("writer");

            DoNext = doNext;

            Action = () => {
                Write(writer);
                writer.Flush();
            };
        }

        public abstract void Write(IHttpWriter writer);
    }
}