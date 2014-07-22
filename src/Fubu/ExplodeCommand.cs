
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using Bottles;
using Bottles.Diagnostics;
using Bottles.PackageLoaders.Assemblies;
using FubuCore;
using FubuCore.CommandLine;
using FubuMVC.Core;
using FubuMVC.Core.Packaging;

namespace Fubu
{
    public class ExplodeInput
    {
        [Description("The root directory of the FubuMVC application")]
        public string Directory { get; set; }

        [Description("If chosen, deletes the fubu-content folder under the root")]
        public bool CleanFlag { get; set; }
    }

    [CommandDescription("Explodes assembly bottle content in a FubuMVC application directory")]
    public class ExplodeCommand : FubuCommand<ExplodeInput>
    {
        public override bool Execute(ExplodeInput input)
        {
            var system = new FileSystem();

            if (input.CleanFlag)
            {
                var contentFolder = input.Directory.AppendPath(FubuMvcPackageFacility.FubuContentFolder);
                Console.WriteLine("Cleaning contents of folder " + contentFolder);
                system.CleanDirectory(contentFolder);
            }

            
            FubuMvcPackageFacility.PhysicalRootPath = input.Directory.ToFullPath();
            PackageRegistry.GetApplicationDirectory = FubuMvcPackageFacility.GetApplicationPath;

            Console.WriteLine("Exploding bottle content at " + FubuMvcPackageFacility.GetApplicationPath().ToFullPath());

            PackageRegistry.LoadPackages(_ => _.Loader(new FubuModuleExploder(input.Directory)));

            return true;
        }
    }

    public class FubuModuleExploder : IPackageLoader
    {
        private readonly string _directory;

        public FubuModuleExploder(string directory)
        {
            _directory = directory.ToFullPath();
        }

        public IEnumerable<IPackageInfo> Load(IPackageLog log)
        {
            var list = new List<string> { _directory };
            var binPath = _directory.AppendPath("bin");
            if (Directory.Exists(binPath))
            {
                list.Add(binPath);

                var releasePath = binPath.AppendPath("release");
                if (Directory.Exists(releasePath))
                {
                    list.Add(releasePath);
                }
                else
                {
                var debugPath = binPath.AppendPath("debug");
                    if (Directory.Exists(debugPath))
                    {
                        list.Add(debugPath);
                    }
                }


            }


            list.Each(x =>
            {
                Console.WriteLine("Looking for assemblies marked with the [FubuModule] attribute in " + x);
            });

            return LoadPackages(list);
        }

        public static IEnumerable<IPackageInfo> LoadPackages(List<string> list)
        {
            return FindAssemblies(list)
                .Select(assem => {
                    Console.WriteLine("Exploding content from " + assem.GetName());
                    var pak = new AssemblyPackageInfo(assem);
                    pak.ForFolder(BottleFiles.WebContentFolder, x => {
                        // nothing, just forcing it to do something
                    });
                    return pak;
                });
        }

        public static IEnumerable<Assembly> FindAssemblies(IEnumerable<string> directories)
        {
            return directories.SelectMany(
                x =>
                AssembliesFromPath(x, assem => assem.GetCustomAttributes(typeof(FubuModuleAttribute), false).Any()));
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
                Assembly assembly =
                    AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(
                        x => x.GetName().Name == Path.GetFileNameWithoutExtension(assemblyPath));

                if (assembly == null)
                {
                    try
                    {
                        assembly = Assembly.LoadFrom(assemblyPath);
                    }
                    catch
                    {
                    }
                }




                if (assembly != null && assemblyFilter(assembly))
                {
                    yield return assembly;
                }
            }
        }
    }

}