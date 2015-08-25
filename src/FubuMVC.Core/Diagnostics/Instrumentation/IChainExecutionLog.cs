using System;

namespace FubuMVC.Core.Diagnostics.Instrumentation
{
    public interface IChainExecutionLog
    {
        void StartSubject(ISubject subject);
        void FinishSubject();
        void LogException(Exception ex);
        void AddLog(object log);
        void Trace(string description, Action action);
    }
}