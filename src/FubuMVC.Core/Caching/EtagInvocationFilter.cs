using System;
using System.Collections.Generic;
using System.Net;
using FubuCore.Binding;
using FubuMVC.Core.Http;
using FubuMVC.Core.Http.Headers;
using FubuMVC.Core.Resources.Etags;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Caching
{
    public class EtagInvocationFilter : IBehaviorInvocationFilter
    {
        private readonly IHeadersCache _cache;
        private readonly Func<ServiceArguments, string> _hashing;

        public EtagInvocationFilter(IHeadersCache cache, Func<ServiceArguments, string> hashing)
        {
            _cache = cache;
            _hashing = hashing;
        }

        public EtagInvocationFilter(IHeadersCache headerCache)
            : this(headerCache, args => args.Get<ICurrentChain>().ResourceHash())
        {
            
        }

        public DoNext Filter(ServiceArguments arguments)
        {
            string etag = null;

            arguments.Get<IRequestData>().ValuesFor(RequestDataSource.Header)
                .Value(HttpRequestHeaders.IfNoneMatch, value => etag = value.RawValue as string);

            if (etag == null) return DoNext.Continue;

            var resourceHash = _hashing(arguments);
            var headers = _cache.Current(resourceHash);
            var currentEtag = headers.ValueFor(HttpResponseHeader.ETag);

            if (etag != currentEtag) return DoNext.Continue;


            var writer = arguments.Get<IHttpWriter>();
            writer.WriteResponseCode(HttpStatusCode.NotModified);

            headers.Each(x => x.Replay(writer));

            return DoNext.Stop;
        }
    }
}