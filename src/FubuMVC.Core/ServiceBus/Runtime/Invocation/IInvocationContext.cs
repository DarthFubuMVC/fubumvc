using System.Collections.Generic;

namespace FubuMVC.Core.ServiceBus.Runtime.Invocation
{
    /// <summary>
    /// Models a queue of outgoing messages as a result of the current message so you don't even try to 
    /// send replies until the original message succeeds
    /// Plus giving you the ability to set the correlation identifiers
    /// </summary>
    public interface IInvocationContext
    {
        /// <summary>
        /// Register a message to be sent via the service bus
        /// as a result of the current message succeeding
        /// </summary>
        /// <param name="message"></param>
        void EnqueueCascading(object message);

        IEnumerable<object> OutgoingMessages();
        Envelope Envelope { get; }

        IContinuation Continuation { get; set; }
    }
}