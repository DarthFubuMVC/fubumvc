using System.Collections.Generic;

namespace FubuMVC.Core.Packaging
{
    public interface IPackageActivator
    {
        void Activate(IEnumerable<PackageInfo> packages);
    }
}