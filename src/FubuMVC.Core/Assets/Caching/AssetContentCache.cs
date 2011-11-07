using System;
using System.Collections.Generic;
using System.Threading;
using FubuCore.Util;
using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Caching;
using FubuMVC.Core.Http;
using FubuMVC.Core.Resources.Etags;

namespace FubuMVC.Core.Assets.Caching
{
    public interface IAssetContentCache
    {
        void LinkFilesToResource(string resourceHash, IEnumerable<AssetFile> files);
        void FlushAll();
    }

    public class AssetContentCache : IEtagCache, IOutputCache, IAssetFileChangeListener, IAssetContentCache
    {
        private readonly Cache<AssetFile, IList<string>> _fileToResourceLinks = new Cache<AssetFile, IList<string>>(file => new List<string>());
        private readonly Cache<string, IRecordedOutput> _outputs = new Cache<string, IRecordedOutput>();
        private readonly ReaderWriterLock _lock = new ReaderWriterLock();

        public void LinkFilesToResource(string resourceHash, IEnumerable<AssetFile> files)
        {
            files.Each(x => _fileToResourceLinks[x].Fill(resourceHash));
        }

        public void FlushAll()
        {
            write = () => _outputs.ClearAll();
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

        public void Changed(AssetFile file)
        {
            write = () => _fileToResourceLinks[file].Each(_outputs.Remove);
        }

        public string Current(string resourceHash)
        {
            return read(() =>
            {
                if (!_outputs.Has(resourceHash))
                {
                    return null;
                }

                // Can be null;
                return _outputs[resourceHash].GetHeaderValue(HttpResponseHeaders.ETag);
            });
        }

        public void Register(string resourceHash, string etag)
        {
            
        }

        public void Eject(string resourceHash)
        {
            throw new NotSupportedException();
        }
    }
}