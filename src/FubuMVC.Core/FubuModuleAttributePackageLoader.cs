using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Bottles;
using Bottles.Diagnostics;
using Bottles.PackageLoaders.Assemblies;

namespace FubuMVC.Core
{
    public class FubuModuleAttributePackageLoader : IPackageLoader
    {
        public IEnumerable<IPackageInfo> Load(IPackageLog log)
        {
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            log.Trace("Looking for assemblies marked with the [FubuModule] attribute in " + baseDirectory);

            return AssembliesFromPath(baseDirectory,
                                      assem => assem.GetCustomAttributes(typeof (FubuModuleAttribute), false).Any())
                .Select(AssemblyPackageInfo.CreateFor);
        }

        // TODO -- this is so common here and in FubuMVC, just get something into FubuCore
        public static IEnumerable<Assembly> AssembliesFromPath(string path, Predicate<Assembly> assemblyFilter)
        {
            var assemblyPaths = Directory.GetFiles(path)
                .Where(file =>
                       Path.GetExtension(file).Equals(
                           ".exe",
                           StringComparison.OrdinalIgnoreCase)
                       ||
                       Path.GetExtension(file).Equals(
                           ".dll",
                           StringComparison.OrdinalIgnoreCase));

            foreach (string assemblyPath in assemblyPaths)
            {
                Assembly assembly = null;
                try
                {
                    assembly = Assembly.LoadFrom(assemblyPath);
                }
                catch
                {
                }

                if (assembly != null && assemblyFilter(assembly))
                {
                    yield return assembly;
                }
            }
        }
    }
}