using System.Collections.Generic;

namespace FubuMVC.Core.Packaging
{
    public interface IPackageActivator
    {
        void Activate(IEnumerable<IPackageInfo> packages, IPackageLog log);
    }
}