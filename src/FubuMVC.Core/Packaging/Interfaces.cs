using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace FubuMVC.Core.Packaging
{
    // TODO -- Needs to be idempotent
    // TODO -- diagnostics?  recording?
    // This guy is going to be responsible for tracking and governing what can happen


    public interface IPackageLoadingConfiguration
    {
        void Assembly(Assembly assembly);
        void Bootstrapper(IBootstrapper bootstrapper);
        void Loader(IPackageLoader loader);
        void Facility(PackageFacility facility);
        void Activator(IPackageActivator activator);
    }

    public class PackageLoadingConfiguration : IPackageLoadingConfiguration
    {
        private readonly IList<Action<PackagingRuntimeGraph>> _configurations = new List<Action<PackagingRuntimeGraph>>();

        private Action<PackagingRuntimeGraph> configure
        {
            set
            {
                _configurations.Add(value);
            }
        }

        public void Assembly(Assembly assembly)
        {
            throw new NotImplementedException();
        }

        public void Bootstrapper(IBootstrapper bootstrapper)
        {
            throw new NotImplementedException();
        }

        public void Loader(IPackageLoader loader)
        {
            throw new NotImplementedException();
        }

        public void Facility(PackageFacility facility)
        {
            throw new NotImplementedException();
        }

        public void Activator(IPackageActivator activator)
        {
            throw new NotImplementedException();
        }

        public void Configure(PackagingRuntimeGraph graph)
        {
            throw new NotImplementedException();
        }
    }

    public static class PackageLoadingConfigurationExtensions
    {
        public static void Bootstrapper<T>(this IPackageLoadingConfiguration configuration) where T : IBootstrapper, new()
        {
            configuration.Bootstrapper(new T());
        }

        public static void Loader<T>(this IPackageLoadingConfiguration configuration) where T : IPackageLoader, new()
        {
            configuration.Loader(new T());
        }

        public static void Activator<T>(this IPackageLoadingConfiguration configuration) where T : IPackageActivator, new()
        {
            configuration.Activator(new T());
        }

        public static void Facility<T>(this IPackageLoadingConfiguration configuration) where T : PackageFacility, new()
        {
            configuration.Facility(new T());
        }
    }

    public interface IPackageLoader
    {
        IEnumerable<IPackageInfo> Load();
    }

    public interface IBootstrapper
    {
        IEnumerable<IPackageActivator> Bootstrap();
    }

    public class PackageFacility : IPackageLoadingConfiguration
    {
        public void Assembly(Assembly assembly)
        {
            throw new NotImplementedException();
        }

        public void Bootstrapper(IBootstrapper bootstrapper)
        {
            throw new NotImplementedException();
        }

        public void Loader(IPackageLoader loader)
        {
            throw new NotImplementedException();
        }

        public void Facility(PackageFacility facility)
        {
            throw new NotImplementedException();
        }

        public void Activator(IPackageActivator activator)
        {
            throw new NotImplementedException();
        }
    }

    public class AssemblyPackageLoader : IPackageLoader
    {
        private readonly Assembly _assembly;

        public AssemblyPackageLoader(Assembly assembly)
        {
            _assembly = assembly;
        }

        public IEnumerable<IPackageInfo> Load()
        {
            throw new NotImplementedException();
        }

        public bool Equals(AssemblyPackageLoader other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other._assembly, _assembly);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (AssemblyPackageLoader)) return false;
            return Equals((AssemblyPackageLoader) obj);
        }

        public override int GetHashCode()
        {
            return (_assembly != null ? _assembly.GetHashCode() : 0);
        }
    }
}