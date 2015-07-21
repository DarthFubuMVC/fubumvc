using System;
using System.Threading.Tasks;
using FubuCore;

namespace FubuMVC.Core.ServiceBus
{
    public class RequestOptions
    {
        public TimeSpan Timeout = 10.Minutes();
        public Uri Destination = null;
    }

    public interface IServiceBus
    {
        /// <summary>
        /// Loosely-coupled Request/Reply pattern
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="request"></param>
        /// <param name="options">Override the timeout and destination</param>
        /// <returns></returns>
        Task<TResponse> Request<TResponse>(object request, RequestOptions options = null);

        void Send<T>(T message);

        /// <summary>
        /// Send to a specific destination rather than running the routing rules
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="destination">The destination to send to</param>
        /// <param name="message"></param>
        void Send<T>(Uri destination, T message);

        /// <summary>
        /// Invoke consumers for the relevant messages managed by the current
        /// service bus instance. This happens immediately and on the current thread.
        /// Error actions will not be executed and the message consumers will not be retried
        /// if an error happens.
        /// </summary>
        void Consume<T>(T message);

        void DelaySend<T>(T message, DateTime time);
        void DelaySend<T>(T message, TimeSpan delay);

        /// <summary>
        /// Send a message and await an acknowledgement that the 
        /// message has been processed
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        /// <returns></returns>
        Task SendAndWait<T>(T message);

        /// <summary>
        /// Send a message to a specific destination and await an acknowledgment
        /// that the message has been processed.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="destination">The destination to send to</param>
        /// <param name="message"></param>
        /// <returns></returns>
        Task SendAndWait<T>(Uri destination, T message);

        /// <summary>
        /// Unsubscribes from any messages this node was locally subscribed to.
        /// </summary>
        Task RemoveSubscriptionsForThisNodeAsync();
    }
}
