using System.ComponentModel;
using FubuMVC.Core.ServiceBus.Runtime;
using FubuMVC.Core.ServiceBus.Runtime.Invocation;

namespace FubuMVC.Core.ServiceBus.ErrorHandling
{
    [Description("Sends a custom response back to the message sender on failure")]
    public class RespondWithMessageContinuation : IContinuation
    {
        private readonly object _message;

        public RespondWithMessageContinuation(object message)
        {
            _message = message;
        }

        public object Message
        {
            get { return _message; }
        }

        public void Execute(Envelope envelope, IEnvelopeContext context)
        {
            context.SendOutgoingMessages(envelope, new[] { Message });
        }
    }
}