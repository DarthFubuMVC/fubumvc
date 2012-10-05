using System.Collections.Generic;
using System.Net;
using FubuMVC.Core.Assets.Caching;
using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Http.Headers;
using FubuMVC.Core.Resources.Etags;
using FubuMVC.Core.Runtime;
using System.Linq;

namespace FubuMVC.Core.Assets.Http
{
    public class AssetWriter
    {
        private readonly IAssetContentCache _cache;
        private readonly IETagGenerator<IEnumerable<AssetFile>> _eTagGenerator;
        private readonly IOutputWriter _output;
        private readonly IAssetCacheHeaders _cachingHeaders;
        private readonly IContentWriter _writer;

        public AssetWriter(IAssetContentCache cache, IContentWriter writer, IETagGenerator<IEnumerable<AssetFile>> eTagGenerator, IOutputWriter output, IAssetCacheHeaders cachingHeaders)
        {
            _cache = cache;
            _writer = writer;
            _eTagGenerator = eTagGenerator;
            _output = output;
            _cachingHeaders = cachingHeaders;
        }

        [UrlPattern("get__content")]
        public void Write(AssetPath path)
        {
            var files = _writer.Write(path);
            if (files.Any())
            {
                processAssetFiles(path, files);
            }
            else
            {
                _output.WriteResponseCode(HttpStatusCode.NotFound);
                _output.Write("Cannot find asset " + path.ToFullName());
            }

            
        }

        private void processAssetFiles(AssetPath path, IEnumerable<AssetFile> files)
        {
            var etag = _eTagGenerator.Create(files);

            _cache.LinkFilesToResource(path.ResourceHash, files);

            _output.AppendHeader(HttpResponseHeader.ETag, etag);

            if (!FubuMode.InDevelopment())
            {
                _cachingHeaders.HeadersFor(files).Each(x => x.Write(_output));
            }
        }
    }
}