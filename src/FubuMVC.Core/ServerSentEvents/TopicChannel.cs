namespace FubuMVC.Core.ServerSentEvents
{
    public interface ITopicChannel<T> where T : Topic
    {
        IChannel<T> Channel { get; }
        IEventQueue<T> Queue { get; }
        void WriteEvents(T topic, params IServerEvent[] events);
    }

    public class TopicChannel<T> : ITopicChannel<T> where T : Topic
    {
        public TopicChannel(IEventQueue<T> queue)
        {
            Channel = new Channel<T>(queue);
            Queue = queue;
        }

        public IChannel<T> Channel { get; private set;}
        public IEventQueue<T> Queue { get; private set;}

        public void WriteEvents(T topic, params IServerEvent[] events)
        {
            Channel.Write(q => q.Write(events));
        }

        public void Flush()
        {
            Channel.Flush();
        }
    }
}