using System.Collections.Generic;

namespace FubuMVC.Core.Packaging
{
    public interface IBootstrapper
    {
        IEnumerable<IActivator> Bootstrap(IPackageLog log);
    }
}