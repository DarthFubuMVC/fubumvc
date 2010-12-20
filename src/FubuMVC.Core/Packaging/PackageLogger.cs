using System;
using System.Collections.Generic;

namespace FubuMVC.Core.Packaging
{
    public class PackageLogger : IPackageLogger
    {
        public void WriteAssembliesNotFound(AssemblyFiles theAssemblyFiles, PackageManifest manifest, CreatePackageInput input)
        {
            Console.WriteLine("Did not locate all designated assemblies at {0}", input.PackageFolder);
            Console.WriteLine("Looking for these assemblies in the package manifest file:");
            manifest.AssemblyNames.Each(name => Console.WriteLine("  " + name));
            Console.WriteLine("But only found");
            theAssemblyFiles.Files.Each(file => Console.WriteLine("  " + file));
        }
    }
}