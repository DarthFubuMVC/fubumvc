using System;
using System.Collections.Generic;
using System.Reflection;

namespace Bottles.Diagnostics
{
    public interface IBottlingDiagnostics
    {
        //on logging session
        void LogObject(object target, string provenance);
        void LogExecution(object target, Action continuation);
        IPackageLog LogFor(object target);
        void EachLog(Action<object, PackageLog> action);
        bool HasErrors();

        IEnumerable<LogSubject> LogsForSubjectType<T>();

        PerfTimer Timer { get; }
    }
}