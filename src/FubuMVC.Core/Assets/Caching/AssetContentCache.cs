using System;
using System.Collections.Generic;
using System.Threading;
using FubuCore.Util;
using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Caching;
using FubuMVC.Core.Resources.Etags;
using System.Linq;

namespace FubuMVC.Core.Assets.Caching
{
    public interface IAssetContentCache
    {
        void LinkFilesToResource(string resourceHash, IEnumerable<AssetFile> files);
        void FlushAll();
        void Changed(AssetFile file);
    }

    public class AssetContentCache : IAssetContentCache
    {
        private readonly Cache<AssetFile, IList<string>> _fileToResourceLinks =
            new Cache<AssetFile, IList<string>>(file => new List<string>());

        private readonly IHeadersCache _headers;
        private readonly IOutputCache _outputCache;

        private readonly ReaderWriterLock _lock = new ReaderWriterLock();

        public AssetContentCache(IHeadersCache headers, IOutputCache outputCache)
        {
            _headers = headers;
            _outputCache = outputCache;
        }

        private Action write
        {
            set
            {
                try
                {
                    _lock.AcquireWriterLock(2000);
                    value();
                }
                finally
                {
                    _lock.ReleaseWriterLock();
                }
            }
        }

        public void LinkFilesToResource(string resourceHash, IEnumerable<AssetFile> files)
        {
            files.Each(x => _fileToResourceLinks[x].Fill(resourceHash));
        }

        public void FlushAll()
        {
            write = () =>
            {
                _fileToResourceLinks.SelectMany(x => x).Distinct().Each(eject);
            };
        }

        public void Changed(AssetFile file)
        {
            write = () => _fileToResourceLinks[file].Each(eject);
        }

        private void eject(string hash)
        {
            _headers.Eject(hash);
            _outputCache.Eject(hash);
        }
    }
}