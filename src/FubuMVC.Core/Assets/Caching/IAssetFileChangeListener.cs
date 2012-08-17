using System;
using FubuCore.Logging;
using FubuMVC.Core.Assets.Files;

namespace FubuMVC.Core.Assets.Caching
{
    public interface IAssetFileChangeListener
    {
        void Changed(AssetFile file);
    }

    public class AssetFileChangeListener : IAssetFileChangeListener
    {
        private readonly IAssetContentCache _cache;
        private readonly ILogger _logger;

        public AssetFileChangeListener(IAssetContentCache cache, ILogger logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public void Changed(AssetFile file)
        {
            _logger.InfoMessage(() => new AssetFileChangeDetected{
                Name = file.Name,
                Fullpath = file.FullPath
            });

            _cache.Changed(file);
        }
    }

    public class AssetFileChangeDetected : LogRecord
    {
        public string Name { get; set; }
        public string Fullpath { get; set; }

        public bool Equals(AssetFileChangeDetected other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Name, Name) && Equals(other.Fullpath, Fullpath);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (AssetFileChangeDetected)) return false;
            return Equals((AssetFileChangeDetected) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Name != null ? Name.GetHashCode() : 0)*397) ^ (Fullpath != null ? Fullpath.GetHashCode() : 0);
            }
        }

        public override string ToString()
        {
            return string.Format("Name: {0}, Fullpath: {1}", Name, Fullpath);
        }
    }
}