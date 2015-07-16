using System;
using System.Collections.Generic;
using System.Reflection;
using Bottles.Environment;

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

        public PerfTimer Timer { get; private set; }

        // Hokey, but it gets the job done.
        public static string GetTypeName(object target)
        {
            if (target is IActivator) return typeof (IActivator).Name;
            if (target is Assembly) return typeof (Assembly).Name;
            if (target is IEnvironmentRequirement) return typeof (IEnvironmentRequirement).Name;

            return target.GetType().Name;
        }
    }
}