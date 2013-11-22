using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Bottles.Diagnostics;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.Registration;

namespace FubuMVC.Core
{
    /// <summary>
    /// Helper class to find and apply concrete IFubuRegistryExtension classes in Bottle assemblies
    /// </summary>
    public static class FubuExtensionFinder
    {
        public static void ApplyExtensions(FubuRegistry registry, IEnumerable<Assembly> assemblies, IPackageLog packageLog)
        {
            FindAllExtensionTypes(assemblies).Select(type => typeof (Importer<>).CloseAndBuildAs<IImporter>(type)).Each(
                x => x.Apply(registry, packageLog));
        }

        public static IEnumerable<Type> FindAllExtensionTypes(IEnumerable<Assembly> assemblies)
        {
            if (!assemblies.Any()) return new Type[0];

            var pool = new TypePool {IgnoreExportTypeFailures = false};
            pool.AddAssemblies(assemblies);

            // Yeah, it really does have to be this way
            return pool.TypesMatching(
                t =>
                hasDefaultCtor(t) && t.GetInterfaces().Any(i => i.FullName == typeof (IFubuRegistryExtension).FullName) && !t.HasAttribute<DoNotAutoImportAttribute>());
        }

        private static bool hasDefaultCtor(Type type)
        {
            return type.GetConstructor(new Type[0]) != null;
        }

        public interface IImporter
        {
            void Apply(FubuRegistry registry, IPackageLog packageLog);
        }

        public class Importer<T> : IImporter where T : IFubuRegistryExtension, new()
        {
            public void Apply(FubuRegistry registry, IPackageLog packageLog)
            {
                packageLog.Trace("Applying extension " + typeof(T).FullName);
                registry.Import<T>();
            }
        }
    }
}