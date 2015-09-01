using System.ComponentModel;
using FubuMVC.Core.ServiceBus.Runtime;
using FubuMVC.Core.ServiceBus.Runtime.Invocation;

namespace FubuMVC.Core.ServiceBus.ErrorHandling
{
    [Description("Requeue the envelope locally")]
    public class RequeueContinuation : IContinuation
    {
        public void Execute(Envelope envelope, IEnvelopeContext context)
        {
            envelope.Callback.Requeue();
        }
    }
}