using System;
using System.Net;
using System.Reflection;
using FubuCore.Reflection;
using FubuMVC.Core.Continuations;
using FubuMVC.Core.Http;
using FubuMVC.Core.Http.Headers;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Resources.Etags
{
    public class IfNoneMatchNode : ActionCallBase
    {
        private static readonly string _methodName =
            ReflectionHelper.GetProperty<ETagHandler<IfNoneMatchNode>>(x => x.Matches(null)).Name;

        public IfNoneMatchNode(Type resourceType) : base()
        {
            var handlerType = typeof (ETagHandler<>).MakeGenericType(resourceType);
            var method = handlerType.GetMethod(_methodName);

            setHandlerAndMethod(handlerType, method);
        }

        public void SetETagGenerator(object handler)
        {
            
        }

        public void SetETagGeneratorType(Type etagHandlerType)
        {
            
        }

        public override BehaviorCategory Category
        {
            get { return BehaviorCategory.Process; }
        }
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