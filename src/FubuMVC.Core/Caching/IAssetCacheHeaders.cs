using System.Collections.Generic;
using FubuMVC.Core.Http.Headers;

namespace FubuMVC.Core.Caching
{
    public interface IAssetCacheHeaders
    {
        IEnumerable<Header> Headers();
    }
}