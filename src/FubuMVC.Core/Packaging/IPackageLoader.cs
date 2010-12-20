using System.Collections.Generic;

namespace FubuMVC.Core.Packaging
{
    public interface IPackageLoader
    {
        // TODO -- Really, really need to get the log here
        IEnumerable<IPackageInfo> Load();
    }
}