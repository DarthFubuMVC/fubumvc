using System.Collections.Generic;
using System.Web.Hosting;
using Bottles;
using Bottles.Diagnostics;
using FubuCore;

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

            packages.Each(x =>
            {
                x.ForFolder(BottleFiles.WebContentFolder, directory =>  
                {
                    log.Trace("Adding the bottle directory {0} to the virtual directory provider", directory);
                    provider.RegisterContentDirectory(directory);
                }
            );
            });
        }

        public override string ToString()
        {
            return "Adding package web content folders to the virtual path provider ({0})".ToFormat(GetType().Name);
        }
    }
}