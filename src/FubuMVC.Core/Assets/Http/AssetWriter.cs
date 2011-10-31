using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Assets.Caching;
using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Http.Headers;
using FubuMVC.Core.Resources.Etags;

namespace FubuMVC.Core.Assets.Http
{
    public class AssetWriter
    {
        private readonly IAssetContentCache _cache;
        private readonly IETagGenerator<IEnumerable<AssetFile>> _eTagGenerator;
        private readonly IContentWriter _writer;

        public AssetWriter(IAssetContentCache cache, IContentWriter writer,
                           IETagGenerator<IEnumerable<AssetFile>> eTagGenerator)
        {
            _cache = cache;
            _writer = writer;
            _eTagGenerator = eTagGenerator;
        }

        [UrlPattern("get__content")]
        public HttpHeaderValues Write(AssetPath path)
        {
            var files = _writer.Write(path);
            var etag = _eTagGenerator.Create(files);

            _cache.LinkFilesToResource(path.ResourceHash, files);

            return HttpHeaderValues.ForETag(etag);
        }
    }

    public class AssetFileEtagGenerator : IETagGenerator<IEnumerable<AssetFile>>
    {
        public string Create(IEnumerable<AssetFile> target)
        {
            return target.Select(x => x.FullPath).HashByModifiedDate();
        }
    }
}