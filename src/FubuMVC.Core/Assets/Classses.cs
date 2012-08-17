using System.Collections.Generic;
using System.Net;
using FubuCore;
using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Http.Headers;

namespace FubuMVC.Core.Assets
{
    public interface IAssetCacheHeaders
    {
        IEnumerable<Header> Headers(IEnumerable<AssetFile> files);
    }

    public class AssetCacheHeaders : IAssetCacheHeaders
    {
        public int MaxAgeInSeconds = 24*60*60;

        public IEnumerable<Header> Headers(IEnumerable<AssetFile> files)
        {
            // max-age to 24 hours
            yield return new Header(HttpResponseHeader.CacheControl, "private, max-age={0}".ToFormat(MaxAgeInSeconds));
        }
    }
}