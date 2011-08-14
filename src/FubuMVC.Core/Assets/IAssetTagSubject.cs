using FubuMVC.Core.Assets.Files;

namespace FubuMVC.Core.Assets
{
    public interface IAssetTagSubject
    {
        string Name { get; }
        AssetFolder AssetFolder { get; }
    }
}