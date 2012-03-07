using System.Net;
using FubuCore.Binding;
using FubuMVC.Core.Http;
using FubuMVC.Core.Resources.Etags;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Assets.Http
{
    public class AssetEtagInvocationFilter : IBehaviorInvocationFilter
    {
        private readonly IEtagCache _cache;

        public AssetEtagInvocationFilter(IEtagCache cache)
        {
            _cache = cache;
        }

        public DoNext Filter(ServiceArguments arguments)
        {
            string etag = null;

            arguments.Get<IRequestHeaders>().Value<string>(HttpRequestHeaders.IfNoneMatch, value => etag = value);

            if (etag == null) return DoNext.Continue;

            var resourceHash = arguments.Get<ICurrentChain>().ResourceHash();
            var currentEtag = _cache.Current(resourceHash);

            if (etag != currentEtag) return DoNext.Continue;


            arguments.Get<IHttpWriter>().WriteResponseCode(HttpStatusCode.NotModified);
            return DoNext.Stop;
        }
    }
}