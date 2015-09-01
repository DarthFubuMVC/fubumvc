using System;
using System.ComponentModel;
using FubuMVC.Core.ServiceBus.ErrorHandling;
using FubuMVC.Core.ServiceBus.Logging;

namespace FubuMVC.Core.ServiceBus.Runtime.Invocation
{
    [Description("The handler chain was successful, dequeues the envelope")]
    public class ChainSuccessContinuation : IContinuation
    {
        private readonly IInvocationContext _context;

        public ChainSuccessContinuation(IInvocationContext context)
        {
            _context = context;
        }

        public void Execute(Envelope envelope, IEnvelopeContext context)
        {
            try
            {
                context.SendOutgoingMessages(envelope, _context.OutgoingMessages());

                envelope.Callback.MarkSuccessful();

                var message = new MessageSuccessful { Envelope = envelope.ToToken() };
                if (!message.Envelope.IsDelayedEnvelopePollingJobRelated())
                    context.InfoMessage(message);
            }
            catch (Exception ex)
            {
                context.SendFailureAcknowledgement(envelope, "Sending cascading message failed: " + ex.Message);
                context.Error(envelope.CorrelationId, ex.Message, ex);
                envelope.Callback.MoveToErrors(new ErrorReport(envelope, ex));
            }
        }

        public IInvocationContext Context
        {
            get { return _context; }
        }
    }
}