using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Hosting;
using System.Web.Routing;
using Bottles;
using Bottles.Exploding;
using FubuCore;
using FubuMVC.Core.Content;
using FubuMVC.Core.Packaging.VirtualPaths;
using FubuMVC.Core.Registration;

namespace FubuMVC.Core.Packaging
{
    public class FubuMvcPackageFacility : PackageFacility
    {
        public static readonly string FubuPackagesFolder = "fubu-packages";
        public static readonly string FubuContentFolder = "fubu-content";

        private readonly IContentFolderService _contentFolderService = new ContentFolderService(new FileSystem());
        private readonly IMimeTypeProvider _mimeTypeProvider = new DefaultMimeTypeProvider();

        public FubuMvcPackageFacility()
        {
            string applicationPath = GetApplicationPath();

            if (applicationPath.IsNotEmpty())
            {
                var fileSystem = new FileSystem();

                // Development mode
                Loader(new DebugPackageManifestReader(applicationPath, fileSystem, folder => folder));
                
                // Production mode with zip files and standalone assemblies (e.g., Spark.Web.FubuMVC)
                var zipFilePackageReader = BuildZipFilePackageLoader(applicationPath, fileSystem);
                Loader(zipFilePackageReader);

            	var standaloneLoader = BuildStandaloneAssemblyPackageLoader(fileSystem);
				Loader(standaloneLoader);
            }


            Activator(new VirtualPathProviderActivator());
            Activator(new PackageFolderActivator(_contentFolderService));
        }

		public static StandaloneAssemblyPackageLoader BuildStandaloneAssemblyPackageLoader(FileSystem fileSystem)
		{
			var assemblyFinder = new StandaloneAssemblyFinder(fileSystem);
			return new StandaloneAssemblyPackageLoader(assemblyFinder);
		}

        public static ZipFilePackageLoader BuildZipFilePackageLoader(string applicationPath, FileSystem fileSystem)
        {
            var zipFileManifestReader = new DebugPackageManifestReader(applicationPath, fileSystem, ZipFilePackageLoader.GetContentFolderForPackage);
            PackageExploder packageExploder = PackageExploder.GetPackageExploder(fileSystem);
            return new ZipFilePackageLoader(zipFileManifestReader, packageExploder);
        }



        public static string GetApplicationPath()
        {
            return PhysicalRootPath ?? HostingEnvironment.ApplicationPhysicalPath ?? determineApplicationPathFromAppDomain();
        }

        private static string determineApplicationPathFromAppDomain()
        {
            var basePath = AppDomain.CurrentDomain.BaseDirectory;
            if (basePath.EndsWith("bin"))
            {
                basePath = basePath.Substring(0, basePath.Length - 3).TrimEnd(Path.DirectorySeparatorChar);
            }

            return basePath;
        }

        public static string PhysicalRootPath { get; set; }

        public void AddPackagingContentRoutes(ICollection<RouteBase> routes)
        {
            new FileRouteHandler(_contentFolderService, ContentType.images, _mimeTypeProvider).RegisterRoute(routes);
            new FileRouteHandler(_contentFolderService, ContentType.scripts, _mimeTypeProvider).RegisterRoute(routes);
            new FileRouteHandler(_contentFolderService, ContentType.styles, _mimeTypeProvider).RegisterRoute(routes);
        }


        public void RegisterServices(IServiceRegistry services)
        {
            services.AddService<IContentFolderService>(_contentFolderService);
            services.AddService<IMimeTypeProvider>(_mimeTypeProvider);
        }

        public override string ToString()
        {
            return "FubuMVC Packaging Facility";
        }
    }
}