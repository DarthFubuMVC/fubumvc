using System.ComponentModel;

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
            // TODO -- do more here.  There's a GH issue for this.
        }
    }
}