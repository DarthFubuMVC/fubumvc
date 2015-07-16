using FubuMVC.Core.Services.Messaging;

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