using Bottles;
using FubuMVC.Core.Registration;

namespace FubuMVC.Core.Packaging
{
    public class PackagingServiceRegistry : ServiceRegistry
    {
        public PackagingServiceRegistry()
        {
            SetServiceIfNone<IPackageFiles, PackageFilesCache>();
            AddService<IActivator, PackageFileActivator>();
        }
    }
}