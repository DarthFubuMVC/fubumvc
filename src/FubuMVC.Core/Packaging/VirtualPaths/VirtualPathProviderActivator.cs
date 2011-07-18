using System.Collections.Generic;
using System.Web.Hosting;
using Bottles;
using Bottles.Diagnostics;
using FubuCore;
using System.Linq;

namespace FubuMVC.Core.Packaging.VirtualPaths
{
    public class VirtualPathProviderActivator : IActivator
    {
        public void Activate(IEnumerable<IPackageInfo> packages, IPackageLog log)
        {
            if (!HostingEnvironment.IsHosted)
            {
                return;
            }
            
            var provider = new FileSystemVirtualPathProvider();

            HostingEnvironment.RegisterVirtualPathProvider(provider);

            //TODO: Need to search packages in a deterministic order so that packages can override other packages. Sorting by name as a temporary measure.
            packages.OrderBy(x => x.Name).Each(x => x.ForFolder(BottleFiles.WebContentFolder, provider.RegisterContentDirectory));
        }

        public override string ToString()
        {
            return "Adding package web content folders to the virtual path provider ({0})".ToFormat(GetType().Name);
        }
    }
}