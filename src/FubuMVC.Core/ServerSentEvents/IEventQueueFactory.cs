namespace FubuMVC.Core.ServerSentEvents
{
    public interface IEventQueueFactory
    {
        IEventQueue<T> BuildFor<T>(T topic) where T : Topic;
    }

    public interface IEventQueueFactory<in T> where T : Topic
    {
        IEventQueue<T> BuildFor(T topic);
    }
}