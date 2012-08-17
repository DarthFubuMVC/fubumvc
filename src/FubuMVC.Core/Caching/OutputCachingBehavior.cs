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
        private readonly IEtagCache _etagCache;
        private readonly IOutputWriter _writer;
        private readonly IResourceHash _hash;

        public OutputCachingBehavior(IActionBehavior inner, IOutputCache cache, IOutputWriter writer,
                                     IResourceHash hash, IEtagCache etagCache) : base(inner)
        {
            _cache = cache;
            _writer = writer;
            _hash = hash;
            _etagCache = etagCache;
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
            newOutput.ForHeader(HttpResponseHeaders.ETag, etag => _etagCache.Register(resourceHash, etag));

            return newOutput;
        }
    }
}