using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Http;
using FubuMVC.Core.Http.Headers;
using FubuMVC.Core.Resources.Etags;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Caching
{
    public class OutputCachingBehavior : IActionBehavior
    {
        private readonly IOutputCache _cache;
        private readonly ICurrentChain _currentChain;
        private readonly IEtagCache _etagCache;
        private readonly IAssetCacheHeaders _headers;
        private readonly IActionBehavior _inner;
        private readonly IOutputWriter _writer;

        public OutputCachingBehavior(IActionBehavior inner, IOutputCache cache, IOutputWriter writer,
                                     ICurrentChain currentChain, IEtagCache etagCache, IAssetCacheHeaders headers)
        {
            _inner = inner;
            _cache = cache;
            _writer = writer;
            _currentChain = currentChain;
            _etagCache = etagCache;
            _headers = headers;

            Invoker = x => x.Invoke();
            PartialInvoker = x => x.InvokePartial();
        }

        public Action<IActionBehavior> Invoker { get; private set; }
        public Action<IActionBehavior> PartialInvoker { get; private set; }

        public void Invoke()
        {
            generateOutput(Invoker);
        }

        public void InvokePartial()
        {
            generateOutput(PartialInvoker);
        }

        public virtual IRecordedOutput CreateOuput(string resourceHash, Action<IActionBehavior> innerInvocation)
        {
            var newOutput = _writer.Record(() => innerInvocation(_inner));

            newOutput.ForHeader(HttpResponseHeaders.ETag, etag =>
            {
                var headers = new List<Header>(new[] { new Header( HttpResponseHeader. ETag, etag) });

                if (!FubuMode.InDevelopment())
                {
                    IEnumerable<Header> additionalHeaders = _headers.Headers();
                    headers.AddRange(additionalHeaders);
                    additionalHeaders.Each(x => _writer.AppendHeader(x.Name, x.Value));
                }

                _etagCache.Register(resourceHash, etag, headers);

            });

            return newOutput;
        }

        private void generateOutput(Action<IActionBehavior> innerInvocation)
        {
            var resourceHash = _currentChain.ResourceHash();

            var output = _cache.Retrieve(resourceHash, () => CreateOuput(resourceHash, innerInvocation));

            _writer.Replay(output);
        }
    }
}