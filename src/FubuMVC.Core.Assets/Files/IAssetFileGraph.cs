using System.Collections.Generic;

namespace FubuMVC.Core.Assets.Files
{
    public interface IAssetFileGraph
    {
        AssetFile Find(string path);
        AssetPath AssetPathOf(AssetFile file);
        AssetFile FindByPath(string path);
        IEnumerable<AssetFile> AllFiles();
        AssetFile Find(AssetPath path);
        IEnumerable<PackageAssets> AllPackages { get; }
    }
}