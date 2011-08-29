using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FubuMVC.Core.Runtime
{
    public class ResponseCaching : IResponseCaching
    {
        private readonly HttpCachePolicy _cache;
        private readonly HttpResponse _response;

        public ResponseCaching()
        {
            _response = HttpContext.Current.Response;
            _cache = _response.Cache;
        }

        public void CacheRequestAgainstFileChanges(IEnumerable<string> localFiles)
        {
            _response.AddFileDependencies(localFiles.ToArray());

            _cache.VaryByParams["files"] = true;
            _cache.SetLastModifiedFromFileDependencies();
            _cache.SetETagFromFileDependencies();
            _cache.SetCacheability(HttpCacheability.Public);
        }

        public void CacheRequestAgainstFileChanges(string file)
        {
            CacheRequestAgainstFileChanges(new[]{file});
        }
    }
}