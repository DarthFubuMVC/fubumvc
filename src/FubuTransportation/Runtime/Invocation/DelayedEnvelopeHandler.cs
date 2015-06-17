using System;
using System.ComponentModel;
using FubuCore.Dates;
using FubuTransportation.Runtime.Delayed;

namespace FubuTransportation.Runtime.Invocation
{
    [Description("DelayedResponse Message Handler")]
    public class DelayedEnvelopeHandler : SimpleEnvelopeHandler
    {
        private readonly ISystemTime _systemTime;

        public DelayedEnvelopeHandler(ISystemTime systemTime)
        {
            _systemTime = systemTime;
        }

        public override bool Matches(Envelope envelope)
        {
            return envelope.IsDelayed(_systemTime.UtcNow());
        }

        public override void Execute(Envelope envelope, ContinuationContext context)
        {
            try
            {
                envelope.Callback.MoveToDelayedUntil(envelope.ExecutionTime.Value);
                context.Logger.InfoMessage(() => new DelayedEnvelopeReceived { Envelope = envelope.ToToken() });
            }
            catch (Exception e)
            {
                envelope.Callback.MarkFailed(e);
                context.Logger.Error(envelope.CorrelationId, "Failed to move delayed message to the delayed message queue", e);
            }
        }
    }
}