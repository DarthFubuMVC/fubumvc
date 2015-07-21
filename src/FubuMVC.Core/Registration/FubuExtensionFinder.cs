using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.Diagnostics.Packaging;

namespace FubuMVC.Core.Registration
{
    /// <summary>
    /// Helper class to find and apply concrete IFubuRegistryExtension classes in Bottle assemblies
    /// </summary>
    public static class FubuExtensionFinder
    {
        public static IEnumerable<IImporter> FindAllExtensions(this Assembly assembly, IActivationDiagnostics diagnostics)
        {
            // Yeah, it really does have to be this way
            // TODO -- use TypeRepository here.
            var log = diagnostics.LogFor(assembly);
            return assembly.GetExportedTypes().Where(isExtension).Select(type => typeof(Importer<>).CloseAndBuildAs<IImporter>(log,type));
        }


        private static bool isExtension(Type type)
        {
            return type.CanBeCastTo<IFubuRegistryExtension>()
                   && !type.IsOpenGeneric()
                   && type.IsConcreteWithDefaultCtor()
                   && !type.HasAttribute<DoNotAutoImportAttribute>();
        }


        public interface IImporter
        {
            void Apply(FubuRegistry registry);
        }

        public class Importer<T> : IImporter where T : IFubuRegistryExtension, new()
        {
            private readonly IActivationLog _log;

            public Importer(IActivationLog log)
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