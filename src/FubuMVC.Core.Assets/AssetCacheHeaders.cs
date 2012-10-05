using System.Collections.Generic;
using System.Net;
using FubuCore;
using FubuCore.Dates;
using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Http.Headers;

namespace FubuMVC.Core.Assets
{
    public class AssetCacheHeaders : IAssetCacheHeaders
    {
        private readonly ISystemTime _systemTime;
        public int MaxAgeInSeconds = 24*60*60;

        public AssetCacheHeaders(ISystemTime systemTime)
        {
            _systemTime = systemTime;
        }

        public IEnumerable<Header> HeadersFor(IEnumerable<AssetFile> files)
        {
            // max-age to 24 hours
            yield return new Header(HttpResponseHeader.CacheControl, "private, max-age={0}".ToFormat(MaxAgeInSeconds));


            // expires
            yield return new Header(HttpResponseHeader.Expires, _systemTime.UtcNow().AddSeconds(MaxAgeInSeconds).ToString("R"));
        }
    }
}