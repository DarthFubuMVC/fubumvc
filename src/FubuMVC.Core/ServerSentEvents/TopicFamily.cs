using FubuCore.Util;

namespace FubuMVC.Core.ServerSentEvents
{
    public interface ITopicFamily
    {
        void Flush();
    }

    public class TopicFamily<T> : ITopicFamily where T : Topic
    {
        private readonly Cache<T, TopicChannel<T>> _channels = new Cache<T, TopicChannel<T>>();

        public TopicFamily(IEventQueueFactory<T> factory)
        {
            _channels.OnMissing = topic =>
            {
                var queue = factory.BuildFor(topic);

                return new TopicChannel<T>(queue);
            };
        }

        public TopicChannel<T> ChannelFor(T topic)
        {
            return _channels[topic];
        }

        public void SpinUpChannel(T topic)
        {
            _channels.FillDefault(topic);
        }

        public void Flush()
        {
            _channels.Each(x => x.Flush());
            _channels.ClearAll();
        }
    }
}