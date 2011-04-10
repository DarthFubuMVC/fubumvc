using Bottles;

namespace FubuMVC.Core.Packaging
{
    public static class PackageLoadingConfigurationExtensions
    {
        public static void Bootstrapper<T>(this IPackageFacility configuration) where T : IBootstrapper, new()
        {
            configuration.Bootstrapper(new T());
        }

        public static void Loader<T>(this IPackageFacility configuration) where T : IPackageLoader, new()
        {
            configuration.Loader(new T());
        }

        public static void Activator<T>(this IPackageFacility configuration) where T : IActivator, new()
        {
            configuration.Activator(new T());
        }

        public static void Facility<T>(this IPackageFacility configuration) where T : PackageFacility, new()
        {
            configuration.Facility(new T());
        }
    }
}