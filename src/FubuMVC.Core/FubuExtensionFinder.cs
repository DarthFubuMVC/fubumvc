using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Bottles;
using FubuMVC.Core.Packaging;
using FubuMVC.Core.Registration;
using FubuCore.Reflection;
using System.Collections.Generic;
using FubuCore;

namespace FubuMVC.Core
{
    public static class FubuExtensionFinder
    {
        public static void ApplyExtensions(FubuRegistry registry, IEnumerable<Assembly> assemblies)
        {
            FindAllExtensionTypes(assemblies).Select(type => typeof (Importer<>).CloseAndBuildAs<IImporter>(type)).Each(
                x => x.Apply(registry));
        }

        public interface IImporter
        {
            void Apply(FubuRegistry registry);
        }

        public class Importer<T> : IImporter where T : IFubuRegistryExtension, new()
        {
            public void Apply(FubuRegistry registry)
            {
                registry.Import<T>();
            }
        }

        public static IEnumerable<Type> FindAllExtensionTypes(IEnumerable<Assembly> assemblies)
        {
            if (!assemblies.Any()) return new Type[0];

            var pool = new TypePool();

            pool.AddAssemblies(assemblies);

            // Yeah, it really does have to be this way
            return pool.TypesMatching(
                t =>
                hasDefaultCtor(t) && t.GetInterfaces().Any(i => i.FullName == typeof (IFubuRegistryExtension).FullName));
        }

        private static bool hasDefaultCtor(Type type)
        {
            return type.GetConstructor(new Type[0]) != null;
        }
    }
}