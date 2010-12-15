using System;
using System.Collections.Generic;
using System.Web.Hosting;
using FubuCore;

namespace FubuMVC.Core.Packaging
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

            packages.Each(x => x.ForFolder(FubuMvcPackages.WebContentFolder, provider.RegisterContentDirectory));
        }

        public override string ToString()
        {
            return "Adding package web content folders to the virtual path provider ({0})".ToFormat(GetType().Name);
        }
    }
}