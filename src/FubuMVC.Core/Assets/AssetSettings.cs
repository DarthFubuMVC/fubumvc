using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using FubuCore;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Runtime.Files;

namespace FubuMVC.Core.Assets
{
    public class AssetSettings
    {
        // This is tested through integration tests
        public Task<AssetGraph> Build(IFubuApplicationFiles files)
        {
            return Task.Factory.StartNew(() => {
                var graph = new AssetGraph();

                var search = CreateAssetSearch();

                graph.Add(files.FindFiles(search).Select(x => new Asset(x)));

                Aliases.AllKeys.Each(alias => graph.StoreAlias(alias, Aliases[alias]));

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

        public readonly NameValueCollection Aliases = new NameValueCollection();
    }
}