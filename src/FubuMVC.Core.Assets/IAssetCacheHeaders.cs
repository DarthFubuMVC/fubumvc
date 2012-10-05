using System.Collections.Generic;
using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Http.Headers;

namespace FubuMVC.Core.Assets
{
    public interface IAssetCacheHeaders
    {
        IEnumerable<Header> HeadersFor(IEnumerable<AssetFile> files);
    }
}