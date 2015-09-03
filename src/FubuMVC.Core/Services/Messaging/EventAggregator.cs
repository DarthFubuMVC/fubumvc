using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using FubuMVC.Core.Services.Messaging.Tracking;

namespace FubuMVC.Core.Services.Messaging
{
    public static class EventAggregator
    {
        private static readonly BlockingCollection<object> _messages;
        private static IRemoteListener _remoteListener;
        private static CancellationTokenSource _cancellationSource;
        private static readonly IMessagingHub _messaging = new MessagingHub();

        static EventAggregator()
        {
            _messages = new BlockingCollection<object>(new ConcurrentQueue<object>());
        }

        public static IMessagingHub Messaging
        {
            get { return _messaging; }
        }

        public static void Start(IRemoteListener remoteListener)
        {
            _remoteListener = remoteListener;

            _cancellationSource = new CancellationTokenSource();
            Task.Factory.StartNew(read, _cancellationSource.Token);
        }

        private static void read()
        {
            foreach (object o in _messages.GetConsumingEnumerable(_cancellationSource.Token))
            {
                // TODO -- should this be async as well?  Or assume that the remote listener will handle it?
                var json = JsonSerialization.ToJson(o);
                _remoteListener.Send(json);

                // TODO -- send to a local messaging hub?
            }
        }

        public static void Stop()
        {
            if (_cancellationSource != null) _cancellationSource.Cancel();
        }

        public static void SendMessage(string category, string message)
        {
            SendMessage(new ServiceMessage
            {
                Category = category,
                Message = message
            });
        }

        public static void SendMessage<T>(T message)
        {
            try
            {
                _messages.Add(message);
                _messaging.Send(message);
            }
            catch (Exception e)
            {
                // THIS IS IMPORTANT, NO FAILURES CAN POSSIBLY GET OUT HERE
                Console.WriteLine(e);
            }
        }

        public static void ReceivedMessage(object message)
        {
            SendMessage(MessageTrack.ForReceived(message));
        }

        public static void SentMessage(object message)
        {
            SendMessage(MessageTrack.ForSent(message));
        }
    }
}