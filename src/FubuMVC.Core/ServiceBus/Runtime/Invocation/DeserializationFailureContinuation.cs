using System.ComponentModel;
using FubuMVC.Core.ServiceBus.ErrorHandling;
using FubuMVC.Core.ServiceBus.Runtime.Serializers;

namespace FubuMVC.Core.ServiceBus.Runtime.Invocation
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

        public void Execute(Envelope envelope, IEnvelopeContext context)
        {
            envelope.Message = null; // Prevent the error from throwing again.
            context.SendFailureAcknowledgement(envelope, "Deserialization failed");
            context.Error(envelope.CorrelationId, _exception.Message, _exception);
            envelope.Callback.MoveToErrors(new ErrorReport(envelope, _exception));
        }
    }
}