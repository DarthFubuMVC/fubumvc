﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Hosting;
using System.Web.Routing;
using FubuCore;
using FubuMVC.Core.Content;
using FubuMVC.Core.Registration;

namespace FubuMVC.Core.Packaging
{
    public class FubuMvcPackageFacility : PackageFacility
    {
        private readonly IContentFolderService _contentFolderService = new ContentFolderService(new FileSystem());
        private readonly IMimeTypeProvider _mimeTypeProvider = new DefaultMimeTypeProvider();
        private readonly IContentCacheBehavior _contentCacheBehavior = new ContentCacheBehavior();

        public FubuMvcPackageFacility()
        {
            string applicationPath = GetApplicationPath();

            if (applicationPath.IsNotEmpty())
            {
                var fileSystem = new FileSystem();

                // Development mode
                Loader(new PackageManifestReader(applicationPath, fileSystem, folder => folder));
                
                // Production mode with zip files and standalone assemblies (e.g., Spark.Web.FubuMVC)
                var zipFilePackageReader = BuildZipFilePackageReader(applicationPath, fileSystem);
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

        public static ZipFilePackageReader BuildZipFilePackageReader(string applicationPath, FileSystem fileSystem)
        {
            var zipFileManifestReader = new PackageManifestReader(applicationPath, fileSystem, ZipFilePackageReader.GetContentFolderForPackage);
            PackageExploder packageExploder = GetPackageExploder(fileSystem);
            return new ZipFilePackageReader(zipFileManifestReader, packageExploder);
        }

        public static PackageExploder GetPackageExploder(FileSystem fileSystem)
        {
            return new PackageExploder(new ZipFileService(), new PackageExploderLogger(x => Console.WriteLine(x)), fileSystem);
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
            new FileRouteHandler(_contentFolderService, ContentType.images, _mimeTypeProvider, _contentCacheBehavior).RegisterRoute(routes);
            new FileRouteHandler(_contentFolderService, ContentType.scripts, _mimeTypeProvider, _contentCacheBehavior).RegisterRoute(routes);
            new FileRouteHandler(_contentFolderService, ContentType.styles, _mimeTypeProvider, _contentCacheBehavior).RegisterRoute(routes);
        }


        public void RegisterServices(IServiceRegistry services)
        {
            services.AddService<IContentFolderService>(_contentFolderService);
            services.AddService<IMimeTypeProvider>(_mimeTypeProvider);
            services.AddService<IContentCacheBehavior>(_contentCacheBehavior);
        }

        public override string ToString()
        {
            return "FubuMVC Packaging Facility";
        }
    }
}