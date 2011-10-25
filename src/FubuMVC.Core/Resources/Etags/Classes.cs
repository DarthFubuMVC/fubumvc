using System.Net;
using FubuMVC.Core.Continuations;
using FubuMVC.Core.Http;
using FubuMVC.Core.Http.Headers;

namespace FubuMVC.Core.Resources.Etags
{
    public interface IEtagCache
    {
        // Can be null
        string CurrentETag(string resourcePath);
        void WriteCurrentETag(string resourcePath, string etag);
    }

    public interface IETagGenerator<T>
    {
        string Create(T target);
    }


    public class ETaggedRequest
    {
        public string IfNoneMatch { get; set; }

        [ResourcePath]
        public string ResourcePath { get; set; }
    }


    public class ETagTuple<T>
    {
        public T Target { get; set; }
        public ETaggedRequest Request { get; set; }
    }

    public class ETagHandler<T>
    {
        private readonly IEtagCache _cache;
        private readonly IETagGenerator<T> _generator;

        public ETagHandler(IEtagCache cache, IETagGenerator<T> generator)
        {
            _cache = cache;
            _generator = generator;
        }

        public FubuContinuation Matches(ETaggedRequest request)
        {
            return _cache.CurrentETag(request.ResourcePath) == request.IfNoneMatch
                       ? FubuContinuation.EndWithStatusCode(HttpStatusCode.NotModified)
                       : FubuContinuation.NextBehavior();
        }

        public HttpHeaderValues CreateETag(ETagTuple<T> tuple)
        {
            var etag = _generator.Create(tuple.Target);
            _cache.WriteCurrentETag(tuple.Request.ResourcePath, etag);

            return new HttpHeaderValues(HttpResponseHeaders.ETag, etag);
        }
    }
}