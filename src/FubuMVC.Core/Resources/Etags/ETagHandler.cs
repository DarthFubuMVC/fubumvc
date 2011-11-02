using System.Net;
using FubuCore;
using FubuMVC.Core.Continuations;

namespace FubuMVC.Core.Resources.Etags
{
    public class ETagHandler<T>
    {
        private readonly IEtagCache _cache;
        private readonly IETagGenerator<T> _generator;

        //, IETagGenerator<T> generator
        public ETagHandler(IEtagCache cache)
        {
            _cache = cache;
            //_generator = generator;
        }

        public FubuContinuation Matches(ETaggedRequest request)
        {
            var current = _cache.Current(request.ResourceHash);
            return current.IsNotEmpty() && current == request.IfNoneMatch
                       ? FubuContinuation.EndWithStatusCode(HttpStatusCode.NotModified)
                       : FubuContinuation.NextBehavior();
        }

        // This needs to be in a different class.  Unit te
        //public HttpHeaderValues CreateETag(ETagTuple<T> tuple)
        //{
        //    var etag = _generator.Create(tuple.Target);
        //    _cache.Register(tuple.Request.ResourceHash, etag);

        //    return new HttpHeaderValues(HttpResponseHeaders.ETag, etag);
        //}
    }
}