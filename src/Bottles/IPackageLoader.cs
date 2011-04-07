using System.Collections.Generic;

namespace Bottles
{
    public interface IPackageLoader
    {
        // TODO -- Really, really need to get the log here
        IEnumerable<IPackageInfo> Load();
    }
}