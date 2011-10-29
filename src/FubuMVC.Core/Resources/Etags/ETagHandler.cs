using System.Net;
using FubuMVC.Core.Continuations;
using FubuMVC.Core.Http;
using FubuMVC.Core.Http.Headers;

namespace FubuMVC.Core.Resources.Etags
{
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
            return _cache.Current(request.ResourceHash) == request.IfNoneMatch
                       ? FubuContinuation.EndWithStatusCode(HttpStatusCode.NotModified)
                       : FubuContinuation.NextBehavior();
        }

        public HttpHeaderValues CreateETag(ETagTuple<T> tuple)
        {
            var etag = _generator.Create(tuple.Target);
            _cache.Register(tuple.Request.ResourceHash, etag);

            return new HttpHeaderValues(HttpResponseHeaders.ETag, etag);
        }
    }
}