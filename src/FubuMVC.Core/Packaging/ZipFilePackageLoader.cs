using System.Collections.Generic;
using System.Linq;
using Bottles;
using Bottles.Diagnostics;
using Bottles.Exploding;
using FubuCore;

namespace FubuMVC.Core.Packaging
{
    public class ZipFilePackageLoader : IPackageLoader
    {
        private readonly IPackageManifestReader _reader;
        private readonly IPackageExploder _exploder;

        public ZipFilePackageLoader(IPackageManifestReader reader, IPackageExploder exploder)
        {
            _reader = reader;
            _exploder = exploder;
        }

        public IEnumerable<IPackageInfo> Load(IPackageLog log)
        {
            var applicationDirectory = FubuMvcPackageFacility.GetApplicationPath();

            //this finds all of the bottles in <applicationDirectory>/bin/packages
            //then calls load from folder on each exploded zip
            return _exploder.ExplodeAllZipsAndReturnPackageDirectories(applicationDirectory, log)
                .Select(dir => _reader.LoadFromFolder(dir));
        }

        public static string GetContentFolderForPackage(string packageFolder)
        {
            return FileSystem.Combine(packageFolder, BottleFiles.WebContentFolder);
        }
    }
}