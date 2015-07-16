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
            if (target is Assembly) return typeof (Assembly).Name;
            if (target is IEnvironmentRequirement) return typeof (IEnvironmentRequirement).Name;

            // TODO -- add IEnvironmentTest

            return target.GetType().Name;
        }
    }
}