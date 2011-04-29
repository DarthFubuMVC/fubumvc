using System;
using System.Collections.Generic;
using System.Linq;
using Bottles.Assemblies;

namespace Bottles.Creation
{
    public class PackageLogger : IPackageLogger
    {
        public void WriteAssembliesNotFound(AssemblyFiles theAssemblyFiles, PackageManifest manifest, CreatePackageInput input)
        {
            Console.WriteLine("Did not locate all designated assemblies at {0}", input.PackageFolder);
            Console.WriteLine("Looking for these assemblies in the package manifest file:");
            manifest.Assemblies.Each(name => Console.WriteLine("  " + name));
            Console.WriteLine("But only found:");
            if(!theAssemblyFiles.Files.Any()) Console.WriteLine("  Found no files");
            theAssemblyFiles.Files.Each(file => Console.WriteLine("  " + file));
        }
    }
}