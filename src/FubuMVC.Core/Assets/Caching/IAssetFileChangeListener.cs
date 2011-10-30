using FubuMVC.Core.Assets.Files;

namespace FubuMVC.Core.Assets.Caching
{
    public interface IAssetFileChangeListener
    {
        void Changed(AssetFile file);
    }
}