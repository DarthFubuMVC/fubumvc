using System;
using System.Collections.Generic;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Diagnostics.Instrumentation
{
    public interface IChainExecutionLog
    {
        void StartSubject(ISubject subject);
        void FinishSubject();
        void LogException(Exception ex);
        void AddLog(object log);
        void Trace(string description, Action action);
        void RecordHeaders(IDictionary<string, object> env);
        void RecordBody(IDictionary<string, object> env);

        Guid Id { get; }
        BehaviorChain RootChain { get; set; }
    }
}