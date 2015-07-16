using System;

namespace Bottles.Diagnostics
{
    public interface IBottlingDiagnostics
    {
        //on logging session
        void LogExecution(object target, Action continuation);
        IPackageLog LogFor(object target);
        void EachLog(Action<object, PackageLog> action);
        bool HasErrors();


        PerfTimer Timer { get; }
    }
}