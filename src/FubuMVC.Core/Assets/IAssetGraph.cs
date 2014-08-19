using System.Collections.Generic;

namespace FubuMVC.Core.Assets
{
    public interface IAssetGraph
    {
        IEnumerable<Asset> Assets { get; }
    }
}