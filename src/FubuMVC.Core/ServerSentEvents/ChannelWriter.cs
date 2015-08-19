using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FubuMVC.Core.Http;
using FubuMVC.Core.Http.Compression;

namespace FubuMVC.Core.ServerSentEvents
{
    [DoNotCompress]
    public class ChannelWriter<T> where T : Topic
    {
        private readonly IHttpRequest _connectivity;
        private readonly IServerEventWriter _writer;
        private readonly ITopicChannelCache _cache;
        private readonly IChannelInitializer<T> _channelInitializer;
        private IChannel<T> _channel;
        private T _topic;

        private TaskCompletionSource<bool> _liveConnection;

        public ChannelWriter(IHttpRequest connectivity, IServerEventWriter writer, ITopicChannelCache cache, IChannelInitializer<T> channelInitializer)
        {
            _connectivity = connectivity;
            _writer = writer;
            _cache = cache;
            _channelInitializer = channelInitializer;
        }

        public Task Write(T topic)
        {
            _liveConnection = new TaskCompletionSource<bool>();
            _topic = topic;
            ITopicChannel<T> topicChannel;

            if (!_cache.TryGetChannelFor(_topic, out topicChannel))
            {
                _liveConnection.SetResult(false);
                return _liveConnection.Task;
            }

            _channel = topicChannel.Channel;

            writeEvents(_channelInitializer.GetInitializationEvents(topic));

            FindEvents();
            return _liveConnection.Task;
        }

        public void FindEvents()
        {
            if (!_connectivity.IsClientConnected() || !_channel.IsConnected())
            {
                _liveConnection.SetResult(false);
                return;
            }

            var task = _channel.FindEvents(_topic);

            OnFaulted(task);
            handleFoundEvents(task);
        }

        private void handleFoundEvents(Task<IEnumerable<IServerEvent>> task)
        {
            var continuation = task.ContinueWith(x =>
            {
                if (x.IsFaulted) return;

                if (!_connectivity.IsClientConnected())
                {
                    _liveConnection.SetResult(false);
                    return;
                }

                writeEvents(x.Result);
                FindEvents();
            }); // Intentionally not attached to parent to prevent stack overflow exceptions.

            OnFaulted(continuation);
        }

        private void writeEvents(IEnumerable<IServerEvent> events)
        {
            var lastSuccessfulMessage = events
                .TakeWhile(y => {
                    if (_connectivity.IsClientConnected())
                    {
                        return _writer.Write(y);
                    }

                    return false;
                })
                .LastOrDefault();

            if (lastSuccessfulMessage != null)
            {
                _topic.LastEventId = lastSuccessfulMessage.Id;
            }
        }

        private void OnFaulted(Task task)
        {
            task.ContinueWith(x =>
            {
                try
                {
                    var aggregateException = x.Exception.Flatten();
                    _liveConnection.SetException(aggregateException.InnerExceptions);
                }
                finally
                {
                    _liveConnection.SetResult(false);
                }
            }, TaskContinuationOptions.OnlyOnFaulted);
        }
    }
}