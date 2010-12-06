using System.Collections.Generic;

namespace FubuMVC.Core.Packaging
{
    public interface IBootstrapper
    {
        IEnumerable<IPackageActivator> Bootstrap(IPackageLog log);
    }
}