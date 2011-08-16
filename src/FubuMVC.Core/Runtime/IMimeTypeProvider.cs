using FubuMVC.Core.Assets.Files;

namespace FubuMVC.Core.Runtime
{
    public interface IMimeTypeProvider
    {
        MimeType For(string extension);
        MimeType For(string extension, AssetFolder folder);
    }
}