using System;
using System.Collections.Generic;
using FubuMVC.Core.Packaging;

namespace Fubu.Packages
{
    public class PackageLogger : IPackageLogger
    {
        public void WriteAssembliesNotFound(CreatePackageInput input, PackageManifest manifest, IEnumerable<string> candidates)
        {
            Console.WriteLine("Did not locate all designated assemblies at {0}", input.PackageFolder);
            Console.WriteLine("Looking for these assemblies in the package manifest file:");
            manifest.AssemblyNames.Each(name => Console.WriteLine("  " + name));
            Console.WriteLine("But only found");
            candidates.Each(file => Console.WriteLine("  " + file));
        }
    }
}