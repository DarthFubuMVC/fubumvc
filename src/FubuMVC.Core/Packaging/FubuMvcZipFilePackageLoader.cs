using Bottles;

namespace FubuMVC.Core.Packaging
{
    public class FubuMvcZipFilePackageLoader : ZipFilePackageLoader
    {
        public FubuMvcZipFilePackageLoader()
            : base(FubuMvcPackageFacility.GetExplodedPackagesDirectory(), FubuMvcPackageFacility.GetPackageDirectories()
                )
        {
        }
    }
}