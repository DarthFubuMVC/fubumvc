using System;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Resources.Etags;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Caching
{
    public class OutputCachingBehavior : WrappingBehavior
    {
        private readonly IOutputCache _cache;
        private readonly IResourceHash _hash;
        private readonly IHeadersCache _headersCache;
        private readonly IOutputWriter _writer;

        public OutputCachingBehavior(IActionBehavior inner, IOutputCache cache, IOutputWriter writer,
                                     IResourceHash hash, IHeadersCache headersCache) : base(inner)
        {
            _cache = cache;
            _writer = writer;
            _hash = hash;
            _headersCache = headersCache;
        }

        protected override void invoke(Action action)
        {
            var resourceHash = _hash.CreateHash();

            var output = _cache.Retrieve(resourceHash, () => CreateOutput(resourceHash, action));

            _writer.Replay(output);
        }

        public virtual IRecordedOutput CreateOutput(string resourceHash, Action invocation)
        {
            var newOutput = _writer.Record(invocation);

            _headersCache.Register(resourceHash, newOutput.Headers());

            return newOutput;
        }
    }
}