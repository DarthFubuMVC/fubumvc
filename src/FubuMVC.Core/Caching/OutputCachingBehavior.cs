using System;
using FubuCore.Logging;
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
        private readonly ILogger _logger;
        private readonly IOutputWriter _writer;

        public OutputCachingBehavior(IOutputCache cache, IOutputWriter writer, IResourceHash hash, IHeadersCache headersCache, ILogger logger)
        {
            _cache = cache;
            _writer = writer;
            _hash = hash;
            _headersCache = headersCache;
            _logger = logger;
        }

        protected override void invoke(Action action)
        {
            var resourceHash = _hash.CreateHash();

            bool hit = true;

            var output = _cache.Retrieve(resourceHash, () =>
            {
                hit = false;
                var returnValue = CreateOutput(resourceHash, action);

                return returnValue;
            });



            log(hit);

            _writer.Replay(output);
        }

        private void log(bool hit)
        {
            if (hit)
            {
                _logger.DebugMessage(() => new CacheHit{Description = _hash.Describe()});
            }
            else
            {
                _logger.DebugMessage(() => new CacheMiss{Description = _hash.Describe()});
            }
        }

        public virtual IRecordedOutput CreateOutput(string resourceHash, Action invocation)
        {
            var newOutput = _writer.Record(invocation);

            _headersCache.Register(resourceHash, newOutput.Headers());

            return newOutput;
        }
    }

    public class CacheHit : LogRecord
    {
        public string Description { get; set; }

        public override string ToString()
        {
            return string.Format("CacheHit: {0}", Description);
        }
    }

    public class CacheMiss : LogRecord
    {
        public string Description { get; set; }

        public override string ToString()
        {
            return string.Format("CacheMiss: {0}", Description);
        }
    }


}