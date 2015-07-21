using System;

namespace FubuMVC.Core.ServiceBus.Runtime.Invocation
{
    /// <summary>
    /// Executes the handler chains for a message in context
    /// with the current message
    /// </summary>
    public interface IMessageExecutor
    {
        /// <summary>
        /// Consumes and executes the message within the same
        /// request and transaction context as the currently
        /// Executing handler. Throws exception if no consumer
        /// found.
        /// </summary>
        /// <param name="message"></param>
        void Execute(object message);

        /// <summary>
        /// Consumes and executes the message within the same
        /// request and transaction context as the currently
        /// Executing handler for messages with a consumer;
        /// when no consumer is found, the onNoConsumer
        /// action is executed
        /// </summary>
        /// <param name="message"></param>
        /// <param name="onNoConsumer"></param>
        /// <returns>Returns true if consumer was found, false
        /// if no consumer found</returns>
        bool TryExecute(object message, Action<object> onNoConsumer);
    }
}