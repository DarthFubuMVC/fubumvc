using Bottles.Services.Messaging;

namespace Fubu.Running
{
    public class Messaging : IMessaging
    {
        public void Send<T>(T message)
        {
            EventAggregator.SendMessage(message);
        }
    }
}