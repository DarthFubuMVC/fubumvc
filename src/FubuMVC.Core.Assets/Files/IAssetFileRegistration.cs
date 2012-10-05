namespace FubuMVC.Core.Assets.Files
{
    public interface IAssetFileRegistration
    {
        void AddFile(AssetPath path, AssetFile file);
    }
}