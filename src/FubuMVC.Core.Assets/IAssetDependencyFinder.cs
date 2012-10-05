using System.Collections.Generic;

namespace FubuMVC.Core.Assets
{
    public interface IAssetDependencyFinder
    {
        IEnumerable<string> CompileDependenciesAndOrder(IEnumerable<string> names);
    }
}