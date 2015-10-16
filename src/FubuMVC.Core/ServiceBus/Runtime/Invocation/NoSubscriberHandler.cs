using System.ComponentModel;
using FubuMVC.Core.ServiceBus.Logging;

namespace FubuMVC.Core.ServiceBus.Runtime.Invocation
{
    [Description("Policies for handling messages that have no registered handlers")]
    public class NoSubscriberHandler : SimpleEnvelopeHandler
    {
        public override bool Matches(Envelope envelope)
        {
            return true;
        }

        public override void Execute(Envelope envelope, IEnvelopeContext context)
        {
            context.SendFailureAcknowledgement(envelope, "No subscriber");
            envelope.Callback.MarkSuccessful();
            context.InfoMessage(() => new NoHandlerForMessage{Envelope = envelope.ToToken()});
        }
    }
}