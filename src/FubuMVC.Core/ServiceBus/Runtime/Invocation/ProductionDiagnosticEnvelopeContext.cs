using FubuCore.Dates;
using FubuCore.Logging;
using FubuMVC.Core.ServiceBus.Runtime.Cascading;

namespace FubuMVC.Core.ServiceBus.Runtime.Invocation
{
    public class ProductionDiagnosticEnvelopeContext : EnvelopeContext
    {
        public ProductionDiagnosticEnvelopeContext(ILogger logger, ISystemTime systemTime, IChainInvoker invoker, IOutgoingSender outgoing) : base(logger, systemTime, invoker, outgoing)
        {
        }
    }
}