using System;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Http;
using FubuMVC.Core.Resources.Etags;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Caching
{
    public class OutputCachingBehavior : WrappingBehavior
    {
        private readonly IOutputCache _cache;
        private readonly ICurrentChain _currentChain;
        private readonly IEtagCache _etagCache;
        private readonly IOutputWriter _writer;

        public OutputCachingBehavior(IActionBehavior inner, IOutputCache cache, IOutputWriter writer,
                                     ICurrentChain currentChain, IEtagCache etagCache) : base(inner)
        {
            _cache = cache;
            _writer = writer;
            _currentChain = currentChain;
            _etagCache = etagCache;
        }

        protected override void invoke(Action action)
        {
            var resourceHash = _currentChain.ResourceHash();

            var output = _cache.Retrieve(resourceHash, () => CreateOutput(resourceHash, action));

            _writer.Replay(output);
        }

        public virtual IRecordedOutput CreateOutput(string resourceHash, Action invocation)
        {
            var newOutput = _writer.Record(invocation);
            newOutput.ForHeader(HttpResponseHeaders.ETag, etag => _etagCache.Register(resourceHash, etag));

            return newOutput;
        }
    }
}