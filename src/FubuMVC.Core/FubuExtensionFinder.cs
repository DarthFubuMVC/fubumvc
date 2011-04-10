using System;
using System.Collections.Generic;
using System.Linq;
using Bottles;
using FubuMVC.Core.Packaging;
using FubuMVC.Core.Registration;

namespace FubuMVC.Core
{
    public static class FubuExtensionFinder
    {
        public static IEnumerable<IFubuRegistryExtension> FindAllExtensions()
        {
            if (!PackageRegistry.PackageAssemblies.Any()) return new IFubuRegistryExtension[0];

            var pool = new TypePool(null){
                ShouldScanAssemblies = true
            };

            pool.AddAssemblies(PackageRegistry.PackageAssemblies);

            // Yeah, it really does have to be this way
            return pool.TypesMatching(
                t =>
                hasDefaultCtor(t) && t.GetInterfaces().Any(i => i.FullName == typeof (IFubuRegistryExtension).FullName))
                .Select(buildExtension);
        }

        private static bool hasDefaultCtor(Type type)
        {
            return type.GetConstructor(new Type[0]) != null;
        }

        private static IFubuRegistryExtension buildExtension(Type type)
        {
            var contextType = Type.GetType(type.AssemblyQualifiedName);
            return (IFubuRegistryExtension) Activator.CreateInstance(contextType);
        }
    }
}