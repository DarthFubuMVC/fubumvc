using System.ComponentModel;
using FubuCore.Logging;
using FubuTransportation.Runtime;
using FubuTransportation.Runtime.Invocation;

namespace FubuTransportation.ErrorHandling
{
    [Description("Requeue the envelope locally")]
    public class RequeueContinuation : IContinuation
    {
        public void Execute(Envelope envelope, ContinuationContext context)
        {
            envelope.Callback.Requeue();
        }
    }
}