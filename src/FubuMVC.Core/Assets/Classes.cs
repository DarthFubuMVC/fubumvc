using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FubuCore;
using FubuCore.Util;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Runtime.Files;

namespace FubuMVC.Core.Assets
{
    public class Asset
    {
        public readonly IFubuFile File;
        public readonly string Url;
        public readonly MimeType MimeType;
        public readonly string Filename;

        // For testing only!
        public Asset(string url)
        {
            Url = url;
            Filename = Path.GetFileName(url);
        }

        public Asset(IFubuFile file)
        {
            File = file;

            Filename = Path.GetFileName(file.Path);
            MimeType = MimeType.MimeTypeByFileName(Filename);

            Url = file.RelativePath.Replace("\\", "/").TrimStart('/');
        }
    }

    public class AssetSettings
    {
        public Task<AssetGraph> Build(IFubuApplicationFiles files)
        {
            return Task.Factory.StartNew(() => {
                var graph = new AssetGraph();

                var search = CreateAssetSearch();

                graph.Add(files.FindFiles(search).Select(x => new Asset(x)));


                return graph;
            });
        }

        public FileSet CreateAssetSearch()
        {
            var extensions = assetMimeTypes().SelectMany(x => x.Extensions).Select(x => "*" + x).Join(";");

            return FileSet.Deep(extensions);
        }

        private IEnumerable<MimeType> assetMimeTypes()
        {
            yield return MimeType.Javascript;
            yield return MimeType.Css;

            foreach (var mimetype in MimeType.All().Where(x => x.Value.StartsWith("image/")))
            {
                yield return mimetype;
            }
        }
    }

    public interface IAssetGraph
    {
        Asset FindAsset(string search);
        IEnumerable<Asset> Assets { get; }
    }

    public class AssetGraph : IAssetGraph
    {
        private readonly IList<Asset> _assets = new List<Asset>();
        private readonly ConcurrentCache<string, Asset> _searches = new ConcurrentCache<string, Asset>();

        public AssetGraph()
        {
            _searches.OnMissing = findAsset;
        }

        public void Add(Asset asset)
        {
            _assets.Add(asset);
        }

        public void Add(IEnumerable<Asset> assets)
        {
            _assets.AddRange(assets);
        }

        public Asset FindAsset(string search)
        {
            return _searches[search];
        }

        private Asset findAsset(string search)
        {
            search = search.TrimStart('/');

            var exact = _assets.FirstOrDefault(x => x.Url == search);
            if (exact != null) return exact;

            if (search.Contains("/"))
            {
                var pathSearch = "/" + search;
                return _assets.FirstOrDefault(x => x.Url.EndsWith(pathSearch));
            }

            return _assets.FirstOrDefault(x => x.Filename == search);
        }

        public IEnumerable<Asset> Assets
        {
            get { return _assets; }
        }
    }
}