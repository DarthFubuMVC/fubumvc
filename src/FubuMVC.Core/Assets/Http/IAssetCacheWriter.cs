using FubuMVC.Core.Assets.Files;

namespace FubuMVC.Core.Assets.Http
{
    public interface IAssetCacheWriter
    {
        void Write(AssetFile file);
    }
}