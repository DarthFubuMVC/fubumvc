using System.Collections.Generic;
using System.Web.Hosting;

namespace FubuMVC.Core.Packaging
{
    public class VirtualPathProviderActivator : IActivator
    {
        public void Activate(IEnumerable<IPackageInfo> packages, IPackageLog log)
        {
            var provider = new FileSystemVirtualPathProvider();
            HostingEnvironment.RegisterVirtualPathProvider(provider);

            packages.Each(x => x.ForFolder(FubuMvcPackages.WebContentFolder, provider.RegisterContentDirectory));
        }
    }
}