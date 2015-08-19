namespace FubuMVC.Core.ServerSentEvents
{
    public class DefaultEventQueueFactory<T> : IEventQueueFactory<T> where T : Topic
    {
        public IEventQueue<T> BuildFor(T topic)
        {
            return new EventQueue<T>();
        }
    }
}