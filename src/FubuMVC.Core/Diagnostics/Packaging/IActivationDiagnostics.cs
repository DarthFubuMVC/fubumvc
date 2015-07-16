using System;

namespace FubuMVC.Core.Diagnostics.Packaging
{
    public interface IActivationDiagnostics
    {
        //on logging session
        void LogExecution(object target, Action continuation);
        IActivationLog LogFor(object target);
        void EachLog(Action<object, ActivationLog> action);
        bool HasErrors();


        PerfTimer Timer { get; }
    }
}