using System;
using System.Collections.Generic;
using System.Reflection;

namespace FubuMVC.Core.Packaging
{
    public interface IPackagingDiagnostics
    {
        void LogObject(object target, string provenance);
        void LogPackage(IPackageInfo package, IPackageLoader loader);
        void LogBootstrapperRun(IBootstrapper bootstrapper, IEnumerable<IPackageActivator> activators);
        void LogAssembly(IPackageInfo package, Assembly assembly, string provenance);
        void LogDuplicateAssembly(IPackageInfo package, string assemblyName);
        void LogAssemblyFailure(IPackageInfo package, string fileName, Exception exception);
        void LogExecution(object target, Action continuation);
        PackageRegistryLog LogFor(object target);
    }

    public static class PackagingDiagnosticsExtensions
    {
        public static void LogExecutionOnEach<T>(this IPackagingDiagnostics diagnostics, IEnumerable<T> targets, Action<T> continuation)
        {
            targets.Each(t =>
            {
                diagnostics.LogExecution(t, () => continuation(t));
            });
        }

        public static void LogPackages(this IPackagingDiagnostics diagnostics, IPackageLoader loader, IEnumerable<IPackageInfo> packages)
        {
            packages.Each(p =>
            {
                diagnostics.LogPackage(p, loader);
            });
        }
    }
}