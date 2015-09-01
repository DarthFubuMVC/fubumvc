using System.Collections.Generic;
using FubuMVC.Core.ServiceBus.Runtime;

namespace FubuMVC.Core.Diagnostics.Instrumentation
{
    public class NulloExecutionLogger : IExecutionLogger
    {
        public void Record(IChainExecutionLog log, IDictionary<string, object> http)
        {
            // no-op
        }

        public void Record(IChainExecutionLog log, Envelope envelope)
        {
            // no-op
        }
    }
}