using System;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Urls;

namespace FubuMVC.Core.Continuations
{
    public class RedirectableHandler<T> : ContinuationHandler where T : class
    {
        private IFubuRequest _request;
        public RedirectableHandler(
            IUrlRegistry registry,
            IOutputWriter writer,
            IFubuRequest request,
            IPartialFactory factory)
            : base(registry, writer, request, factory)
        {
            _request = request;

        }

        protected override DoNext performInvoke()
        {
            Redirectable<T> redirectable = _request.Get<Redirectable<T>>();
            if (redirectable.Model != null) {
                _request.Set<T>(redirectable.Model);
            }

            redirectable.Continuation.Process(this);
            return DoNext.Stop;
        }

    }
}
