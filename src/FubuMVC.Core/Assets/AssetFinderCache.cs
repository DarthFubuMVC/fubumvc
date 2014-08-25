using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FubuCore;
using FubuCore.Util;
using FubuMVC.Core.Packaging;
using FubuMVC.Core.Runtime.Files;

namespace FubuMVC.Core.Assets
{
    public interface IAssetFinder
    {
        Asset FindAsset(string search);
        AssetGraph FindAll();
    }

    public class AssetFinderCache : IAssetFinder
    {
        private readonly AssetSettings _settings;
        private readonly IFubuApplicationFiles _files;
        private readonly ConcurrentCache<string, Asset> _searches = new ConcurrentCache<string, Asset>();
        private readonly Lazy<AssetGraph> _graph; 

        public AssetFinderCache(AssetSettings settings, IFubuApplicationFiles files)
        {
            _settings = settings;
            _files = files;
            _searches.OnMissing = findAsset;

            _graph = new Lazy<AssetGraph>(findAll);
        }

        public Asset FindAsset(string search)
        {
            return _searches[search];
        }

        private Asset findAsset(string search)
        {
            search = search.TrimStart('/');
            var alias = _settings.FileForAlias(search);

            if (alias.IsNotEmpty()) search = alias;
            
            var filename = Path.GetFileName(search);
            var cdn = _settings.FindCdnAsset(search);

            var files = findFiles(filename).ToArray();
            var file = findMatch(files, search);


            if (file != null)
            {
                var asset = new Asset(file);
                if (cdn != null)
                {
                    asset.CdnUrl = cdn.Url;
                    asset.FallbackTest = cdn.Fallback;
                }

                return asset;
            }
            
            if (cdn != null)
            {
                return new Asset(cdn.Url)
                {
                    CdnUrl = cdn.Url,
                    FallbackTest = cdn.Fallback
                };
            }

            return null;
        }

        private IFubuFile findMatch(IEnumerable<IFubuFile> files, string search)
        {
            var exactMatch = files.FirstOrDefault(x => x.RelativePath == search);
            if (exactMatch != null) return exactMatch;

            return files.FirstOrDefault(x => x.RelativePath.EndsWith(search));
        }

        private IEnumerable<IFubuFile> findFiles(string filename)
        {
            if (_settings.Mode == SearchMode.PublicFolderOnly)
            {
                var publicFolder = _settings.DeterminePublicFolder();
                var appFolder = FubuMvcPackageFacility.GetApplicationPath();

                return new FileSystem().FindFiles(publicFolder, FileSet.Deep(filename))
                    .Select(x =>
                    {
                        return new FubuFile(x, ContentFolder.Application)
                        {
                            RelativePath = x.PathRelativeTo(appFolder).Replace('\\', '/')
                        };
                    });
            }


            return _files.FindFiles(FileSet.Deep(filename, _settings.Exclusions));
        } 

        public AssetGraph FindAll()
        {
            return _graph.Value;
        }

        private AssetGraph findAll()
        {
            var graph = new AssetGraph();

            var files = findAssetFiles();

            graph.Add(files.Select(x => new Asset(x)));

            _settings.Aliases.AllKeys.Each(alias => graph.StoreAlias(alias, _settings.Aliases[alias]));

            _settings.CdnAssets.Each(x => graph.RegisterCdnAsset(x));

            return graph;
        }


        private IEnumerable<IFubuFile> findAssetFiles()
        {
            var search = _settings.CreateAssetSearch();

            if (_settings.Mode == SearchMode.PublicFolderOnly)
            {
                var publicFolder = _settings.DeterminePublicFolder();
                var appFolder = FubuMvcPackageFacility.GetApplicationPath();

                return new FileSystem().FindFiles(publicFolder, search)
                    .Select(x =>
                    {
                        return new FubuFile(x, ContentFolder.Application)
                        {
                            RelativePath = x.PathRelativeTo(appFolder).Replace('\\', '/')
                        };
                    });
            }


            return _files.FindFiles(search);
        }
    }
}