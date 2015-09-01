using System.ComponentModel;
using FubuCore;
using FubuMVC.Core.ServiceBus.Logging;

namespace FubuMVC.Core.ServiceBus.Runtime.Invocation
{
    [Description("Publishes the response to the Event Aggregator and removes the message from the queue")]
    public class ResponseEnvelopeHandler : SimpleEnvelopeHandler
    {
        public override bool Matches(Envelope envelope)
        {
            return envelope.ResponseId.IsNotEmpty();
        }

        public override void Execute(Envelope envelope, IEnvelopeContext context)
        {
            context.InfoMessage(() => new MessageSuccessful { Envelope = envelope.ToToken() });
            envelope.Callback.MarkSuccessful();
        }

    }
}