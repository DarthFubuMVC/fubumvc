using System.ComponentModel;
using FubuTransportation.Runtime;
using FubuTransportation.Runtime.Invocation;

namespace FubuTransportation.ErrorHandling
{
    [Description("Retry the message through the handler chain without being re-queued")]
    public class RetryNowContinuation : IContinuation
    {
        public void Execute(Envelope envelope, ContinuationContext context)
        {
            context.Pipeline.Invoke(envelope);
        }
    }
}