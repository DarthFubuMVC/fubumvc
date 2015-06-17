using System.ComponentModel;
using FubuTransportation.Runtime;
using FubuTransportation.Runtime.Invocation;

namespace FubuTransportation.ErrorHandling
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

        public void Execute(Envelope envelope, ContinuationContext context)
        {
            context.SendOutgoingMessages(envelope, new[] { Message });
        }
    }
}