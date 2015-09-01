using System;
using System.Collections.Generic;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.ServiceBus.Runtime;

namespace FubuMVC.Core.Diagnostics.Instrumentation
{
    public interface IChainExecutionLog
    {
        void StartSubject(ISubject subject);
        void FinishSubject();
        void LogException(Exception ex);
        void Log(object log);
        void Trace(string description, Action action);
        void RecordHeaders(IDictionary<string, object> env);
        void RecordBody(IDictionary<string, object> env);

        void RecordHeaders(Envelope envelope);
        void RecordBody(Envelope envelope);

        Guid Id { get; }
        BehaviorChain RootChain { get; set; }
    }
}