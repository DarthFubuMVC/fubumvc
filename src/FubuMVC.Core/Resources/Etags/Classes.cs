using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using FubuCore.Binding;
using FubuCore.Util;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Continuations;
using FubuMVC.Core.Http;
using FubuMVC.Core.Http.Headers;
using FubuMVC.Core.Runtime;
using FubuCore;
using System.Linq;

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

    /*
     *  1. FubuContinuation.EndWithStatusCode()
     *  2. FubuContinuation.AssertWasEndedWithStatusCode()
     *  3. need a property binder for ResourcePath
     *  4.) test that it's registered
     * 
     * 
     * 
     * 
     * 
     */





    public class ETaggedRequest
    {
        public string IfNoneMatch { get; set; }

        [ResourcePath]
        public string ResourcePath { get; set; }
    }

    public class ResourcePathAttribute : BindingAttribute
    {
        public override void Bind(PropertyInfo property, IBindingContext context)
        {
            var resource = context.Service<ICurrentHttpRequest>().RelativeUrl().WithoutQueryString();
            property.SetValue(context.Object, resource, null);
        }
    }



    

    public class ETagTuple<T>
    {
        public T Target { get; set; }
        public ETaggedRequest Request { get; set;}
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

            throw new NotImplementedException();
            //var values = new HttpHeaderValues()
        }
    }


    // Split this monster up?
    public class ETagBehavior<T> : BasicBehavior where T : class
    {
        private readonly IHttpWriter _writer;
        private readonly IEtagCache _cache;
        private readonly IETagGenerator<T> _generator;
        private readonly IRequestHeaders _headers;
        private readonly IFubuRequest _request;
        private readonly ICurrentHttpRequest _httpRequest;


        public ETagBehavior(IHttpWriter writer, IEtagCache cache, IETagGenerator<T> generator, IRequestHeaders headers, IFubuRequest request, ICurrentHttpRequest httpRequest) : base(PartialBehavior.Ignored)
        {
            _writer = writer;
            _cache = cache;
            _generator = generator;
            _headers = headers;
            _request = request;
            _httpRequest = httpRequest;
        }

        protected override DoNext performInvoke()
        {
            var returnValue = DoNext.Continue;

            _headers.Value<string>(HttpRequestHeaders.IfNoneMatch, requestedETag =>
            {
                var resourcePath = _httpRequest.RelativeUrl().WithoutQueryString();
                var latestETag = _cache.CurrentETag(resourcePath);
                if (requestedETag == latestETag)
                {
                    _writer.WriteResponseCode(HttpStatusCode.NotModified);
                    returnValue = DoNext.Stop;
                }
            });

            return returnValue;
        }

        protected override void afterInsideBehavior()
        {
            var subject = _request.Get<T>();
            var etag = _generator.Create(subject);
            _writer.AppendHeader(HttpResponseHeaders.ETag, etag);
            var resourcePath = _httpRequest.RelativeUrl().WithoutQueryString();
            _cache.WriteCurrentETag(resourcePath, etag);
        }
    }
}