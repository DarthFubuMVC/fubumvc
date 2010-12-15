using System.Collections.Generic;

namespace FubuMVC.Core.Packaging
{
    public interface IActivator
    {
        void Activate(IEnumerable<IPackageInfo> packages, IPackageLog log);
    }
}