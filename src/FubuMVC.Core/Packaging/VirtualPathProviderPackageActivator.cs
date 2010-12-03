using System.Collections.Generic;
using System.Web.Hosting;

namespace FubuMVC.Core.Packaging
{
    public class VirtualPathProviderPackageActivator : IPackageActivator
    {
        public void Activate(IEnumerable<PackageInfo> packages, IPackageLog log)
        {
            var provider = new FileSystemVirtualPathProvider();
            HostingEnvironment.RegisterVirtualPathProvider(provider);

            packages.Each(x => provider.RegisterContentDirectory(x.FilesFolder));
        }
    }
}