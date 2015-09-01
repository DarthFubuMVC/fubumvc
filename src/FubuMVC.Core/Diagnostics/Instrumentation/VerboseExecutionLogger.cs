using System.Collections.Generic;
using FubuMVC.Core.ServiceBus.Runtime;

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

        public void Record(IChainExecutionLog log, Envelope envelope)
        {
            log.RecordHeaders(envelope);
            log.RecordBody(envelope);

            _storage.Store(log);
        }
    }
}