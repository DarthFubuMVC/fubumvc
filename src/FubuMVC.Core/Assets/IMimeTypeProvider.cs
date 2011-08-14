using FubuMVC.Core.Assets.Files;

namespace FubuMVC.Core.Assets
{
    public interface IMimeTypeProvider
    {
        string For(string extension);
        void Register(string extension, string mimeType);
        string For(string extension, AssetFolder folder);
    }
}