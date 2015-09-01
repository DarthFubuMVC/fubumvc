using System.ComponentModel;
using FubuMVC.Core.ServiceBus.Runtime;
using FubuMVC.Core.ServiceBus.Runtime.Invocation;

namespace FubuMVC.Core.ServiceBus.ErrorHandling
{
    [Description("Retry the message through the handler chain without being re-queued")]
    public class RetryNowContinuation : IContinuation
    {
        public void Execute(Envelope envelope, IEnvelopeContext context)
        {
            context.Retry(envelope);
        }
    }
}