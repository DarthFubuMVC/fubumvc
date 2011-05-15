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
        public IEnumerable<IPackageInfo> Load(IPackageLog log)
        {
            var exploder = PackageExploder.GetPackageExploder(log);
            var reader = new PackageManifestReader(new FileSystem(), GetContentFolderForPackage);

            return FubuMvcPackageFacility.GetPackageDirectories().SelectMany(dir =>
            {
                return exploder.ExplodeDirectory(new ExplodeDirectory(){
                    DestinationDirectory = FubuMvcPackageFacility.GetExplodedPackagesDirectory(),
                    PackageDirectory = dir,
                    Log = log
                });
            }).Select(dir => reader.LoadFromFolder(dir));
        }

        public static string GetContentFolderForPackage(string packageFolder)
        {
            return FileSystem.Combine(packageFolder, BottleFiles.WebContentFolder);
        }
    }
}