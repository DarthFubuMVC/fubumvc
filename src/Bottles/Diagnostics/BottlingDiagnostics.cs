using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Bottles.Environment;
using FubuCore;

namespace Bottles.Diagnostics
{
    public class BottlingDiagnostics : IBottlingDiagnostics
    {
        private readonly LoggingSession _log;


        
        public BottlingDiagnostics()
        {
            Timer = new PerfTimer();
            _log = new LoggingSession(Timer);
        }

        public void LogAssembly(IPackageInfo package, Assembly assembly, string provenance)
        {
            try
            {
                var versionInfo = getVersion(assembly);


                _log.LogObject(assembly, provenance);
                var packageLog = _log.LogFor(package);
                packageLog.Trace("Loaded assembly '{0}' v{1}".ToFormat(assembly.GetName().FullName,versionInfo.FileVersion));
                packageLog.AddChild(assembly);
            }
            catch (Exception ex)
            {
                throw new Exception("Trying to log assembly '{0}' in package '{1}' at {2}".ToFormat(assembly.FullName, package.Name, assembly.Location), ex);
            }
        }

        private static FileVersionInfo getVersion(Assembly assembly)
        {
            try
            {
                return FileVersionInfo.GetVersionInfo(assembly.Location);
            }
            catch (Exception)
            {
                //grrr
                //blowing up at the moment
                return (FileVersionInfo)Activator.CreateInstance(typeof (FileVersionInfo), BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.CreateInstance, null, new object[]{"name"}, null);
            }
        }

        // just in log to package
        public void LogDuplicateAssembly(IPackageInfo package, string assemblyName)
        {
            _log.LogFor(package).Trace("Assembly '{0}' was ignored because it is already loaded".ToFormat(assemblyName));
        }

        public void LogAssemblyFailure(IPackageInfo package, string fileName, Exception exception)
        {
            var log = _log.LogFor(package);
            log.MarkFailure(exception);
            log.Trace("Failed to load assembly at '{0}'".ToFormat(fileName));
        }


        public void LogObject(object target, string provenance)
        {
            _log.LogObject(target, provenance);
        }

        public void LogExecution(object target, Action continuation)
        {
            _log.LogExecution(target, continuation);
        }

        public IPackageLog LogFor(object target)
        {
            return _log.LogFor(target);
        }

        public void EachLog(Action<object, PackageLog> action)
        {
            _log.EachLog(action);
        }

        public bool HasErrors()
        {
            return _log.HasErrors();
        }

        public IEnumerable<LogSubject> LogsForSubjectType<T>()
        {
            return _log.LogsForSubjectType<T>();
        }

        public PerfTimer Timer { get; private set; }

        // Hokey, but it gets the job done.
        public static string GetTypeName(object target)
        {
            if (target is IActivator) return typeof (IActivator).Name;
            if (target is IPackageInfo) return typeof (IPackageInfo).Name;
            if (target is Assembly) return typeof (Assembly).Name;
            if (target is IEnvironmentRequirement) return typeof (IEnvironmentRequirement).Name;

            // TODO -- add IEnvironmentTest

            return target.GetType().Name;
        }
    }
}