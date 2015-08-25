using System.Collections.Generic;

namespace FubuMVC.Core.Diagnostics.Instrumentation
{
    public class NulloExecutionLogger : IExecutionLogger
    {
        public void Record(IChainExecutionLog log, IDictionary<string, object> http)
        {
            // no-op
        }
    }
}