using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FubuMVC.Core.ServiceBus
{
    /// <summary>
    /// This is a Recording Stub implementation of IServiceBus suitable for
    /// unit testing
    /// </summary>
    public class RecordingServiceBus : IServiceBus
    {
        public readonly IList<object> Sent = new List<object>();
        public readonly IList<object> Consumed = new List<object>();
        public readonly IList<object> Await = new List<object>();
        public readonly IList<DelayedMessage> DelayedSent = new List<DelayedMessage>();
        public readonly IList<SentDirectlyToMessage> SentDirectlyTo = new List<SentDirectlyToMessage>(); 

        public Task<TResponse> Request<TResponse>(object request, RequestOptions options)
        {
            throw new NotSupportedException();
        }

        public void Send<T>(Uri destination, T message)
        {
            SentDirectlyTo.Add(new SentDirectlyToMessage {Destination = destination, Message = message});
        }

        public void Send<T>(T message)
        {
            Sent.Add(message);
        }

        public void Consume<T>(T message)
        {
            Consumed.Add(message);
        }

        public void DelaySend<T>(T message, DateTime time)
        {
            DelayedSent.Add(new DelayedMessage
            {
                Message = message,
                Time = time
            });
        }

        public void DelaySend<T>(T message, TimeSpan delay)
        {
            DelayedSent.Add(new DelayedMessage
            {
                Message = message,
                Delay = delay
            });
        }

        public Task SendAndWait<T>(T message)
        {
            Sent.Add(message);
            return Task.Factory.StartNew(() => Await.Add(message));
        }


        public Task SendAndWait<T>(Uri destination, T message)
        {
            SentDirectlyTo.Add(new SentDirectlyToMessage { Destination = destination, Message = message });
            return Task.Factory.StartNew(() => Await.Add(message));
        }

        public Task RemoveSubscriptionsForThisNodeAsync()
        {
            return Task.FromResult(false);
        }

        public class DelayedMessage
        {
            public TimeSpan Delay;
            public DateTime Time;
            public object Message;
        }

        public class SentDirectlyToMessage
        {
            public Uri Destination;
            public object Message;
        }
    }


}
