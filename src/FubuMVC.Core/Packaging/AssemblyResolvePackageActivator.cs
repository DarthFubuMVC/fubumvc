using System;
using System.Collections.Generic;
using System.Linq;

namespace FubuMVC.Core.Packaging
{
    public class AssemblyResolvePackageActivator : IPackageActivator
    {
        public void Activate(IEnumerable<PackageInfo> packages, IPackageLog log)
        {
            AppDomain.CurrentDomain.AssemblyResolve += (s, args) => OldPackageRegistry.ExtensionAssemblies.FirstOrDefault(assembly =>
            {
                return args.Name == assembly.GetName().Name || args.Name == assembly.GetName().FullName;
            });
        }
    }
}