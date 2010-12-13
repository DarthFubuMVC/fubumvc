using System.Web.Hosting;
using FubuCore;

namespace FubuMVC.Core.Packaging
{
    public class FubuMvcPackageFacility : PackageFacility
    {
        // This is here to redirect the application path in
        // testing scenarios


        public FubuMvcPackageFacility()
        {
            var applicationPath = PhysicalRootPath ?? HostingEnvironment.ApplicationPhysicalPath;

            if (applicationPath.IsNotEmpty())
            {
                Loader(new PackageManifestReader(applicationPath, new FileSystem()));
            }

            // TODO -- should be a package loader for the production mode
            // TODO -- need an activator for scripts/images/styles, etc.

            Activator(new VirtualPathProviderActivator());
        }

        public static string PhysicalRootPath { get; set; }
    }
}