using System.Collections.Generic;
using System.Net;
using FubuCore;
using FubuMVC.Core.Http.Headers;

namespace FubuMVC.Core.Caching
{
    public class AssetCacheHeaders : IAssetCacheHeaders
    {
        public IEnumerable<Header> Headers()
        {
            // max-age to 24 hours
            yield return new Header(HttpResponseHeader.CacheControl,"private, max-age={0}".ToFormat(24*60*60));
        }
    }
}