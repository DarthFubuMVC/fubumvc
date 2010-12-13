using System.Collections.Generic;
using System.Web.Hosting;
using System.Web.Routing;
using FubuCore;
using FubuMVC.Core.Content;

namespace FubuMVC.Core.Packaging
{
    public class FubuMvcPackageFacility : PackageFacility
    {
        private readonly PackagedImageUrlResolver _imageUrlResolver = new PackagedImageUrlResolver(new FileSystem());


        public FubuMvcPackageFacility()
        {
            var applicationPath = PhysicalRootPath ?? HostingEnvironment.ApplicationPhysicalPath;

            if (applicationPath.IsNotEmpty())
            {
                Loader(new PackageManifestReader(applicationPath, new FileSystem()));
            }

            // TODO -- should be a package loader for the production mode
            // TODO -- need an activator for scripts/*/styles, etc.

            Activator(new VirtualPathProviderActivator());
            Activator(new PackageFolderActivator(_imageUrlResolver));
        }

        public static string PhysicalRootPath { get; set; }

        public void AddRoutes(ICollection<RouteBase> routes)
        {
            var imageHandler = new ImageRouteHandler(_imageUrlResolver);
            imageHandler.RegisterRoute(routes);
        }
    }
}