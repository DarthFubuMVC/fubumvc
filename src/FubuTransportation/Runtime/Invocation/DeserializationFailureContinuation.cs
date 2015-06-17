using System;
using System.ComponentModel;
using FubuTransportation.ErrorHandling;
using FubuTransportation.Runtime.Serializers;

namespace FubuTransportation.Runtime.Invocation
{
    [Description("Automatically moves the message to the error queue")]
    public class DeserializationFailureContinuation : IContinuation
    {
        private readonly EnvelopeDeserializationException _exception;

        public DeserializationFailureContinuation(EnvelopeDeserializationException exception)
        {
            _exception = exception;
        }

        public EnvelopeDeserializationException Exception
        {
            get { return _exception; }
        }

        public void Execute(Envelope envelope, ContinuationContext context)
        {
            envelope.Message = null; // Prevent the error from throwing again.
            context.SendFailureAcknowledgement(envelope, "Deserialization failed");
            context.Logger.Error(envelope.CorrelationId, _exception.Message, _exception);
            envelope.Callback.MoveToErrors(new ErrorReport(envelope, _exception));
        }
    }
}