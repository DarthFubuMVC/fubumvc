using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Bottles;
using Bottles.Diagnostics;
using FubuCore;
using FubuCore.Reflection;

namespace FubuMVC.Core
{
    /// <summary>
    /// Helper class to find and apply concrete IFubuRegistryExtension classes in Bottle assemblies
    /// </summary>
    public static class FubuExtensionFinder
    {
        public static IEnumerable<IImporter> FindAllExtensions(this Assembly assembly)
        {
            // Yeah, it really does have to be this way
            var log = PackageRegistry.Diagnostics.LogFor(assembly);
            return assembly.GetExportedTypes().Where(isExtension).Select(type => typeof(Importer<>).CloseAndBuildAs<IImporter>(log,type));
        }

        private static bool isExtension(Type type)
        {
            return type.CanBeCastTo<IFubuRegistryExtension>()
                   && type.IsConcreteWithDefaultCtor()
                   && !type.HasAttribute<DoNotAutoImportAttribute>();
        }


        public interface IImporter
        {
            void Apply(FubuRegistry registry);
        }

        public class Importer<T> : IImporter where T : IFubuRegistryExtension, new()
        {
            private readonly IPackageLog _log;

            public Importer(IPackageLog log)
            {
                _log = log;
            }

            public void Apply(FubuRegistry registry)
            {
                _log.Trace("Applying extension " + typeof(T).FullName);
                registry.Import<T>();
            }
        }
    }
}