using System.Collections.Generic;
using FubuMVC.Core.ServiceBus.Runtime;

namespace FubuMVC.Core.Diagnostics.Instrumentation
{
    public interface IExecutionLogger
    {
        void Record(IChainExecutionLog log, IDictionary<string, object> http);
        void Record(IChainExecutionLog log, Envelope envelope);
    }
}