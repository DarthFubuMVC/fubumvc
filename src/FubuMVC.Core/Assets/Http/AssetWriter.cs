using System.Collections.Generic;
using System.Linq;
using System.Net;
using FubuCore;
using FubuMVC.Core.Assets.Caching;
using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Http.Headers;
using FubuMVC.Core.Resources.Etags;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Assets.Http
{
    public class AssetWriter
    {
        private readonly IAssetContentCache _cache;
        private readonly IETagGenerator<IEnumerable<AssetFile>> _eTagGenerator;
        private readonly IOutputWriter _output;
        private readonly IContentWriter _writer;

        public AssetWriter(IAssetContentCache cache, IContentWriter writer, IETagGenerator<IEnumerable<AssetFile>> eTagGenerator, IOutputWriter output)
        {
            _cache = cache;
            _writer = writer;
            _eTagGenerator = eTagGenerator;
            _output = output;
        }

        [UrlPattern("get__content")]
        public void Write(AssetPath path)
        {
            var files = _writer.Write(path);
            var etag = _eTagGenerator.Create(files);

            _cache.LinkFilesToResource(path.ResourceHash, files);

            _output.AppendHeader(HttpResponseHeader.ETag, etag);
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