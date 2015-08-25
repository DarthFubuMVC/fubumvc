using System.Collections.Generic;

namespace FubuMVC.Core.Diagnostics.Instrumentation
{
    public interface IExecutionLogger
    {
        void Record(IChainExecutionLog log, IDictionary<string, object> http);
    }
}