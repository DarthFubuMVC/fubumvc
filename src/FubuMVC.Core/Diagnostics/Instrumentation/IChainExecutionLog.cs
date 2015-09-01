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


    public class NulloChainExecutionLog : IChainExecutionLog
    {
        public void StartSubject(ISubject subject)
        {
            // nothing
        }

        public void FinishSubject()
        {
            // nothing
        }

        public void LogException(Exception ex)
        {
            // nothing
        }

        public void Log(object log)
        {
            // nothing
        }

        public void Trace(string description, Action action)
        {
            // nothing
            action();
        }

        public void RecordHeaders(IDictionary<string, object> env)
        {
            // nothing
        }

        public void RecordBody(IDictionary<string, object> env)
        {
            // nothing
        }

        public void RecordHeaders(Envelope envelope)
        {
            // nothing
        }

        public void RecordBody(Envelope envelope)
        {
            // nothing
        }

        public Guid Id
        {
            get { return Guid.Empty; }
        }

        public BehaviorChain RootChain { get; set; }
    }
}