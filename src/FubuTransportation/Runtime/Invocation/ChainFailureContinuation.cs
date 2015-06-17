using System;
using FubuCore.Descriptions;
using FubuTransportation.Logging;

namespace FubuTransportation.Runtime.Invocation
{
    public class ChainFailureContinuation : IContinuation, DescribesItself
    {
        private readonly Exception _exception;

        public ChainFailureContinuation(Exception exception)
        {
            _exception = exception;
        }

        public void Execute(Envelope envelope, ContinuationContext context)
        {
            context.SendFailureAcknowledgement(envelope, "Chain execution failed");
            envelope.Callback.MarkFailed(_exception);
            context.Logger.InfoMessage(() => new MessageFailed {Envelope = envelope.ToToken(), Exception = _exception});
            if (envelope.Message == null)
            {
                context.Logger.Error(envelope.CorrelationId, "Error trying to execute a message of type " + envelope.Headers[Envelope.MessageTypeKey], _exception);
            }
            else
            {
                context.Logger.Error(envelope.CorrelationId, envelope.Message.ToString(), _exception);
            }
        }

        public Exception Exception
        {
            get { return _exception; }
        }

        public void Describe(Description description)
        {
            description.Title = "The chain execution failed for this message";
            description.ShortDescription = _exception.GetType().Name;
            description.LongDescription = Exception.ToString();
        }
    }
}