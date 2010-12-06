using System.Collections.Generic;

namespace FubuMVC.Core.Packaging
{
    public interface IPackageLoader
    {
        IEnumerable<IPackageInfo> Load();
    }
}