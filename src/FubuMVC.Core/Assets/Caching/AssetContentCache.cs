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

    public class AssetContentCache : IOutputCache, IAssetContentCache
    {
        private readonly Cache<AssetFile, IList<string>> _fileToResourceLinks =
            new Cache<AssetFile, IList<string>>(file => new List<string>());

        private readonly IHeadersCache _headers;

        private readonly ReaderWriterLock _lock = new ReaderWriterLock();
        private readonly Cache<string, IRecordedOutput> _outputs = new Cache<string, IRecordedOutput>();

        public AssetContentCache(IHeadersCache headers)
        {
            _headers = headers;
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
                _fileToResourceLinks.SelectMany(x => x).Distinct().Each(x => _headers.Eject(x));
                _outputs.ClearAll();
            };
        }

        public void Changed(AssetFile file)
        {
            write = () => _fileToResourceLinks[file].Each(hash =>
            {
                _headers.Eject(hash);
                _outputs.Remove(hash);
            });
        }

        public IRecordedOutput Retrieve(string resourceHash, Func<IRecordedOutput> cacheMiss)
        {
            return read(() =>
            {
                _outputs.Fill(resourceHash, hash => cacheMiss());
                return _outputs[resourceHash];
            });
        }

        private T read<T>(Func<T> findValue)
        {
            try
            {
                _lock.AcquireReaderLock(2000);
                return findValue();
            }
            finally
            {
                _lock.ReleaseReaderLock();
            }
        }
    }
}