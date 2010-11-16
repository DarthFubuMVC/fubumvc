using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using FubuCore;

namespace FubuMVC.Core.Packaging
{
    public class PackageManifestReader
    {
        private readonly string _applicationFolder;
        private readonly IFileSystem _fileSystem;

        public PackageManifestReader(string applicationFolder, IFileSystem fileSystem)
        {
            if (!Path.IsPathRooted(applicationFolder))
            {
                throw new ArgumentOutOfRangeException("applicationFolder", "Only absolute paths can be used here");
            }

            _applicationFolder = applicationFolder;
            _fileSystem = fileSystem;
        }

        public IEnumerable<PackageInfo> ReadAll()
        {
            var includes = _fileSystem.LoadFromFile<PackageIncludeManifest>(_applicationFolder,
                                                                            PackageIncludeManifest.FILE);

            return includes.Folders.Select(f => LoadFromFolder(Path.Combine(_applicationFolder, f)));
        }

        // TODO -- harden this and make it throw the right errors
        public PackageInfo LoadFromFolder(string folder)
        {
            var manifest = _fileSystem.LoadFromFile<PackageManifest>(folder, PackageManifest.FILE);


            var binPath = FileSystem.Combine(_applicationFolder, folder, "bin");
            var assemblies = loadAssembliesFromPath(binPath, manifest.AssemblyNames);

            return new PackageInfo(){
                Assemblies = assemblies,
                FilesFolder = Path.Combine(_applicationFolder, folder).ToFullPath()
            };
        }


        // This was lifted from StructureMap 2.6.2
        // TODO -- harden this
        private IEnumerable<Assembly> loadAssembliesFromPath(string path, IEnumerable<string> assemblyNames)
        {
            IEnumerable<string> assemblyPaths = Directory.GetFiles(path)
                .Where(file =>
                       Path.GetExtension(file).Equals(
                           ".exe",
                           StringComparison.OrdinalIgnoreCase)
                       ||
                       Path.GetExtension(file).Equals(
                           ".dll",
                           StringComparison.OrdinalIgnoreCase));

            var list = new List<Assembly>();

            foreach (string assemblyPath in assemblyPaths)
            {
                var assemblyName = Path.GetFileNameWithoutExtension(assemblyPath);
                if (assemblyNames.Contains(assemblyName))
                {
                    try
                    {
                        Assembly firstAssembly = Assembly.LoadFrom(assemblyPath);
                        list.Add(firstAssembly);
                    }
                    catch
                    {
                    } 
                }


            }

            return list;

        }
    }
}