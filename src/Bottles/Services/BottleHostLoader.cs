using System;
using System.Collections.Generic;
using System.Linq;
using Bottles.Exploding;
using FubuCore;

namespace Bottles.Services
{
    /// <summary>
    /// Packages look like
    /// ~/
    ///     data/
    ///         [assemblies]
    ///     control/
    ///         .manifest
    /// </summary>
    public class BottleHostLoader : 
        IPackageLoader
    {

        private readonly IFileSystem _fileSystem;
        private readonly IPackageExploder _exploder;

        public BottleHostLoader(IFileSystem fileSystem, IPackageExploder exploder)
        {
            _fileSystem = fileSystem;
            _exploder = exploder;
        }

        public IEnumerable<IPackageInfo> Load()
        {
            //<basedir>/packages
            return _exploder.ExplodeAllZipsAndReturnPackageDirectories(AppDomain.CurrentDomain.BaseDirectory)
                .Select(convertToPackage);
        }

        private IPackageInfo convertToPackage(string directory)
        {
            var folder = _fileSystem.GetFullPath(directory);

            var manifest = _fileSystem.LoadFromFile<HostManifest>(folder, HostManifest.CONTROL, HostManifest.FILE);

            var package = new PackageInfo(manifest.Name)
            {
                Description = "{0} ({1})".ToFormat(manifest.Name, folder)
            };

            package.RegisterFolder("data", FileSystem.Combine(folder, HostManifest.DATA));
            package.RegisterFolder("control", FileSystem.Combine(folder, HostManifest.CONTROL));
            
            return package;
        }
    }
}