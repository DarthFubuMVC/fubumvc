using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FubuCore;
using FubuCore.Util;

namespace Bottles.Diagnostics
{
    public class LoggingSession
    {
        private readonly Cache<object, PackageLog> _logs = new Cache<object, PackageLog>(o => new PackageLog{
            Description = o.ToString()
        });

        public void LogObject(object target, string provenance)
        {
            _logs[target].Provenance = provenance;
        }

        public PackageLog LogFor(object target)
        {
            return _logs[target];
        }

        public void LogExecution(object target, Action continuation)
        {
            _logs[target].Execute(continuation);
        }

        public void EachLog(Action<object, PackageLog> action)
        {
            _logs.Each(action);
        }

        public bool HasErrors()
        {
            return _logs.GetAll().Any(x => !x.Success);
        }
    }

    public class PackagingDiagnostics : LoggingSession, IPackagingDiagnostics
    {
        public void LogPackage(IPackageInfo package, IPackageLoader loader)
        {
            LogObject(package, "Loaded by " + loader);
            LogFor(loader).AddChild(package);
        }

        public void LogBootstrapperRun(IBootstrapper bootstrapper, IEnumerable<IActivator> activators)
        {
            var provenance = "Loaded by Bootstrapper:  " + bootstrapper;
            var bootstrapperLog = LogFor(bootstrapper);

            activators.Each(a =>
            {
                LogObject(a, provenance);
                bootstrapperLog.AddChild(a);
            });
        }

        // TODO:  Try to find the assembly file version here. 
        public void LogAssembly(IPackageInfo package, Assembly assembly, string provenance)
        {
            LogObject(assembly, provenance);
            var packageLog = LogFor(package);
            packageLog.Trace("Loaded assembly " + assembly.GetName().FullName);
            packageLog.AddChild(assembly);
        }

        // just in log to package
        public void LogDuplicateAssembly(IPackageInfo package, string assemblyName)
        {
            LogFor(package).Trace("Assembly '{0}' was ignored because it is already loaded".ToFormat(assemblyName));
        }

        public void LogAssemblyFailure(IPackageInfo package, string fileName, Exception exception)
        {
            var log = LogFor(package);
            log.MarkFailure(exception);
            log.Trace("Failed to load assembly at '{0}'".ToFormat(fileName));
        }

        // TODO -- think about this little puppy
        public static string GetTypeName(object target)
        {
            if (target is IBootstrapper) return typeof (IBootstrapper).Name;
            if (target is IActivator) return typeof (IActivator).Name;
            if (target is IPackageLoader) return typeof (IPackageLoader).Name;
            if (target is IPackageFacility) return typeof (IPackageFacility).Name;
            if (target is IPackageInfo) return typeof (IPackageInfo).Name;
            if (target is Assembly) return typeof (Assembly).Name;

            return target.GetType().Name;
        }
    }
}