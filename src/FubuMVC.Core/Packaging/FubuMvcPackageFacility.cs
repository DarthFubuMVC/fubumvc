using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using Bottles;
using Bottles.PackageLoaders;
using Bottles.PackageLoaders.LinkedFolders;
using FubuCore;
using FubuMVC.Core.Packaging.VirtualPaths;

namespace FubuMVC.Core.Packaging
{
    public class FubuMvcPackageFacility : PackageFacility
    {
        public static readonly string FubuPackagesFolder = "fubu-packages";
        public static readonly string FubuContentFolder = "fubu-content";

        public FubuMvcPackageFacility()
        {
            var applicationPath = GetApplicationPath();

            if (applicationPath.IsNotEmpty())
            {
                // Development mode
                Loader(new LinkedFolderPackageLoader(GetApplicationPath(), folder => folder));

                // Production mode with zip files and standalone assemblies (e.g., Spark.Web.FubuMVC)
                Loader(new FubuMvcZipFilePackageLoader());

                var standaloneLoader = new StandaloneAssemblyPackageLoader(new StandaloneAssemblyFinder());
                Loader(standaloneLoader);

                Loader(new FubuModuleAttributePackageLoader());
            }


            Activator(new VirtualPathProviderActivator());
        }

        public static string PhysicalRootPath { get; set; }


        public static string GetApplicationPath()
        {
            return PhysicalRootPath ??
                   HostingEnvironment.ApplicationPhysicalPath ?? determineApplicationPathFromAppDomain();
        }

        public static string FindBinPath()
        {
            var binPath = AppDomain.CurrentDomain.SetupInformation.PrivateBinPath;
            if (binPath.IsNotEmpty())
            {
                return Path.IsPathRooted(binPath)
                    ? binPath
                    : AppDomain.CurrentDomain.SetupInformation.ApplicationBase.AppendPath(binPath);
            }

            return null;
        }

        /// <summary>
        ///   These are the places that FubuMVC should look for packages
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> GetPackageDirectories()
        {
            yield return FileSystem.Combine(GetApplicationPath(), "bin", FubuPackagesFolder);
            yield return FileSystem.Combine(GetApplicationPath(), FubuContentFolder);
        }

        public static string GetExplodedPackagesDirectory()
        {
            return FileSystem.Combine(GetApplicationPath(), FubuContentFolder);
        }

        public static DateTime? Restarted { get; set; }

        private static string determineApplicationPathFromAppDomain()
        {
            var basePath = AppDomain.CurrentDomain.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar);
            if (basePath.EndsWith("bin"))
            {
                basePath = basePath.Substring(0, basePath.Length - 3).TrimEnd(Path.DirectorySeparatorChar);
            }

            var segments = basePath.Split(Path.DirectorySeparatorChar);
            if (segments.Length > 2)
            {
                if (segments[segments.Length - 2].EqualsIgnoreCase("bin"))
                {
                    return basePath.ParentDirectory().ParentDirectory();
                }
            }

            return basePath;
        }

        public override string ToString()
        {
            return "FubuMVC Packaging Facility";
        }
    }
}