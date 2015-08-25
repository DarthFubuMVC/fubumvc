using System.Collections.Generic;

namespace FubuMVC.Core.Diagnostics.Instrumentation
{
    public class VerboseExecutionLogger : IExecutionLogger
    {
        private readonly IExecutionLogStorage _storage;

        public VerboseExecutionLogger(IExecutionLogStorage storage)
        {
            _storage = storage;
        }

        public void Record(IChainExecutionLog log, IDictionary<string, object> http)
        {
            log.RecordHeaders(http);
            log.RecordBody(http);

            _storage.Store(log);
        }
    }
}