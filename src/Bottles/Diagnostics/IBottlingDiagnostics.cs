using System;
using System.Collections.Generic;
using System.Reflection;

namespace Bottles.Diagnostics
{
    public interface IBottlingDiagnostics
    {
        void LogAssembly(IPackageInfo package, Assembly assembly, string provenance);
        void LogDuplicateAssembly(IPackageInfo package, string assemblyName);
        void LogAssemblyFailure(IPackageInfo package, string fileName, Exception exception);


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