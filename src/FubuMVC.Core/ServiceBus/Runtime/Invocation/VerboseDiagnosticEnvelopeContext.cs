using System;
using FubuCore.Dates;
using FubuCore.Logging;
using FubuMVC.Core.Diagnostics.Instrumentation;
using FubuMVC.Core.ServiceBus.Runtime.Cascading;

namespace FubuMVC.Core.ServiceBus.Runtime.Invocation
{
    public class VerboseDiagnosticEnvelopeContext : ProductionDiagnosticEnvelopeContext
    {
        public VerboseDiagnosticEnvelopeContext(ILogger logger, ISystemTime systemTime, IChainInvoker invoker, IOutgoingSender outgoing, IHandlerPipeline pipeline, Envelope envelope, IExecutionLogger executionLogger) : base(logger, systemTime, invoker, outgoing, pipeline, envelope, executionLogger)
        {
        }

        public override void InfoMessage<T>(Func<T> func)
        {
            var message = func();
            _log.Log(message);
            logger.InfoMessage(message);
        }

        public override void DebugMessage<T>(Func<T> func)
        {
            var message = func();
            _log.Log(message);
            logger.DebugMessage(message);
        }

        public override void InfoMessage<T>(T message)
        {
            _log.Log(message);
            base.InfoMessage(message);
        }
    }
}