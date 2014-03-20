using System.Collections.Generic;

namespace FubuMVC.Core.Assets
{
    public interface IAssetGraph
    {
        Asset FindAsset(string search);
        IEnumerable<Asset> Assets { get; }
    }
}