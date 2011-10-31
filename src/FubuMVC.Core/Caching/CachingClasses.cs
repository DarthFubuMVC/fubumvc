using System;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Http;
using FubuMVC.Core.Resources.Etags;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Caching
{
    public class OutputCachingBehavior : IActionBehavior
    {
        private readonly IOutputCache _cache;
        private readonly ICurrentChain _currentChain;
        private readonly IEtagCache _etagCache;
        private readonly IActionBehavior _inner;
        private readonly IOutputWriter _writer;

        public OutputCachingBehavior(IActionBehavior inner, IOutputCache cache, IOutputWriter writer,
                                     ICurrentChain currentChain, IEtagCache etagCache)
        {
            _inner = inner;
            _cache = cache;
            _writer = writer;
            _currentChain = currentChain;
            _etagCache = etagCache;
        }

        public void Invoke()
        {
            generateOutput(x => x.Invoke());
        }

        public void InvokePartial()
        {
            generateOutput(x => x.InvokePartial());
        }

        private void generateOutput(Action<IActionBehavior> generateContent)
        {
            var resourceHash = _currentChain.ResourceHash();
            
            Action contentGeneration = () => generateContent(_inner);
            
            Func<IRecordedOutput> cacheMiss = () =>
            {
                var newOutput =_writer.Record(contentGeneration);
                newOutput.ForHeader(HttpResponseHeaders.ETag, etag => _etagCache.Register(resourceHash, etag));

                return newOutput;
            };

            var output = _cache.Retrieve(resourceHash, cacheMiss);
            _writer.Replay(output);
        }
    }
}