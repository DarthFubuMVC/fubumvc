using FubuCore.Dates;
using FubuCore.Logging;
using FubuMVC.Core.ServiceBus.Runtime.Cascading;

namespace FubuMVC.Core.ServiceBus.Runtime.Invocation
{
    public class VerboseDiagnosticEnvelopeContext : EnvelopeContext
    {
        public VerboseDiagnosticEnvelopeContext(ILogger logger, ISystemTime systemTime, IChainInvoker invoker, IOutgoingSender outgoing, IHandlerPipeline pipeline) : base(logger, systemTime, invoker, outgoing, pipeline)
        {
        }
    }
}