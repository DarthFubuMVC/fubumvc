using System;
using System.IO;
using System.Web.Hosting;
using FubuCore;

namespace FubuMVC.Core
{
    public interface IApplication
    {
        string GetApplicationPath();
        FubuRegistry ToRegistry();
    }

    public class Application<T> : IApplication where T : FubuRegistry, new()
    {
        public readonly T Registry;

        public Application(T registry)
        {
            Registry = registry;
        }

        public Application()
        {
            Registry = new T();
        }

        FubuRegistry IApplication.ToRegistry()
        {
            return Registry;
        }

        public string RootPath;
        //public int Port;
        //public IHost Host;
        //public string Mode;

        // Later, add this as a whilelist override
        //public Assembly[] PackageAssemblies { get; set; }


        public string GetApplicationPath()
        {
            return RootPath ??
                   HostingEnvironment.ApplicationPhysicalPath ?? determineApplicationPathFromAppDomain();
        }

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
    }

    public class BasicApplication : Application<FubuRegistry>
    {
        public BasicApplication(FubuRegistry registry) : base(registry)
        {
        }

        public BasicApplication()
        {
        }
    }
}