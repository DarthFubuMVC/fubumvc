using System.ComponentModel;
using FubuCore;
using FubuCore.Descriptions;
using FubuTransportation.Logging;

namespace FubuTransportation.Runtime.Invocation
{
    [Description("Publishes the response to the Event Aggregator and removes the message from the queue")]
    public class ResponseEnvelopeHandler : SimpleEnvelopeHandler
    {
        public override bool Matches(Envelope envelope)
        {
            return envelope.ResponseId.IsNotEmpty();
        }

        public override void Execute(Envelope envelope, ContinuationContext context)
        {
            context.Logger.InfoMessage(() => new MessageSuccessful { Envelope = envelope.ToToken() });
            envelope.Callback.MarkSuccessful();
        }

    }
}