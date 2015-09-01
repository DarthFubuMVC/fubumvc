using System;
using FubuCore.Dates;
using FubuCore.Logging;
using FubuMVC.Core.Diagnostics.Instrumentation;
using FubuMVC.Core.ServiceBus.Runtime.Cascading;

namespace FubuMVC.Core.ServiceBus.Runtime.Invocation
{
    public class ProductionDiagnosticEnvelopeContext : EnvelopeContext
    {
        private readonly Envelope _envelope;
        private readonly IExecutionLogger _executionLogger;
        protected readonly ChainExecutionLog _log;

        public ProductionDiagnosticEnvelopeContext(ILogger logger, ISystemTime systemTime, IChainInvoker invoker, IOutgoingSender outgoing, IHandlerPipeline pipeline, Envelope envelope, IExecutionLogger executionLogger) : base(logger, systemTime, invoker, outgoing, pipeline)
        {
            _envelope = envelope;
            _executionLogger = executionLogger;
            _log = new ChainExecutionLog();

            _envelope.Log = _log;
        }

        public sealed override void Dispose()
        {
            _log.MarkFinished();
            _executionLogger.Record(_log, _envelope);
        }

        public sealed override void Error(string correlationId, string message, Exception exception)
        {
            _log.LogException(exception);
            base.Error(correlationId, message, exception);
        }
    }
}