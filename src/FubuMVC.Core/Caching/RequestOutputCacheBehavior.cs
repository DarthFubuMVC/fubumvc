using System;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Caching
{
    public class RequestOutputCacheBehavior<TInputModel> : IActionBehavior where TInputModel : class
    {
        private readonly IFubuRequest _request;
        private readonly IRequestOutputCache<TInputModel> _cache;
        private readonly IOutputWriter _writer;
        private readonly IActionBehavior _inner;

        public RequestOutputCacheBehavior(IFubuRequest request, IRequestOutputCache<TInputModel> cache, IOutputWriter writer, IActionBehavior inner)
        {
            _request = request;
            _writer = writer;
            _inner = inner;
            _cache = cache;
        }

        public void Invoke()
        {
            handleInvokation(_inner.Invoke);
        }

        public void InvokePartial()
        {
            handleInvokation(_inner.InvokePartial);
        }

        private void handleInvokation(Action invokation)
        {
            var model = _request.Get<TInputModel>();
            _cache.WithCache(model,
                requestModel => _writer.Record(invokation),
                cachedData => _writer.Write(cachedData.RecordedContentType, cachedData.Content));
        }
    }
}