using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using FubuCore.Binding;
using FubuCore.Util;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Continuations;
using FubuMVC.Core.Http;
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
    
    public class Header
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public Header(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public bool Equals(Header other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Name, Name) && Equals(other.Value, Value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (Header)) return false;
            return Equals((Header) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Name != null ? Name.GetHashCode() : 0)*397) ^ (Value != null ? Value.GetHashCode() : 0);
            }
        }

        public override string ToString()
        {
            return string.Format("Name: {0}, Value: {1}", Name, Value);
        }
    }

    public interface IHaveHeaders
    {
        IEnumerable<Header> Headers { get; }
    }

    public class WriteHeadersBehavior : BasicBehavior
    {
        private readonly IHttpWriter _writer;
        private readonly IFubuRequest _request;

        public WriteHeadersBehavior(IHttpWriter writer, IFubuRequest request) : base(PartialBehavior.Executes)
        {
            _writer = writer;
            _request = request;
        }

        protected override DoNext performInvoke()
        {
            _request.Find<IHaveHeaders>()
                .SelectMany(x => x.Headers)
                .Each(x => _writer.AppendHeader(x.Name, x.Value));

            return DoNext.Continue;
        }
    }


    public class HttpHeaderValues : IHaveHeaders
    {

        private readonly Cache<string, string> _headers = new Cache<string, string>();

        public string this[string key]
        {
            get
            {
                return _headers[key];
            }
            set
            {
                _headers[key] = value;
            }
        }

        public bool Has(string name)
        {
            return _headers.Has(name);
        }


        public IEnumerable<Header> Headers
        {
            get
            {
                foreach (var key in _headers.GetAllKeys())
                {
                    yield return new Header(key, _headers[key]);
                }
            }
        }
    }

    



    public class ETagHandler<T>
    {
        private readonly IEtagCache _cache;
        private readonly IETagGenerator<T> _generator;
        private readonly IFubuRequest _request;

        public ETagHandler(IEtagCache cache, IETagGenerator<T> generator, IFubuRequest request)
        {
            _cache = cache;
            _generator = generator;
            _request = request;
        }

        public FubuContinuation Matches(ETaggedRequest request)
        {
            throw new NotImplementedException();
        }

        public HttpHeaderValues CreateETag(T target)
        {
            var etagRequest = _request.Get<ETaggedRequest>();
            throw new NotImplementedException();
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