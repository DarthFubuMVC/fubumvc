using System;
using System.ComponentModel;
using FubuCore;
using FubuMVC.Core.ServiceBus.Runtime;
using FubuMVC.Core.ServiceBus.Runtime.Invocation;

namespace FubuMVC.Core.ServiceBus.ErrorHandling
{
    [Description("Moves the message envelope to the error queue for the current transport")]
    public class MoveToErrorQueue : IContinuation
    {
        private readonly Exception _exception;

        public MoveToErrorQueue(Exception exception)
        {
            _exception = exception;
        }

        public Exception Exception
        {
            get { return _exception; }
        }

        public void Execute(Envelope envelope, IEnvelopeContext context)
        {
            context.SendFailureAcknowledgement(envelope, "Moved message {0} to the Error Queue.\n{1}".ToFormat(envelope.CorrelationId, _exception));

            var report = new ErrorReport(envelope, _exception);
            envelope.Callback.MoveToErrors(report);
        }
    }
}