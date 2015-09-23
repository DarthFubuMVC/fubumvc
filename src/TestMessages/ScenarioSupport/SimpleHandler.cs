using System.Diagnostics;
using FubuMVC.Core.ServiceBus.Runtime;

namespace TestMessages.ScenarioSupport
{
    public class SimpleHandler<T> where T : Message
    {
        private readonly Envelope _envelope;

        public SimpleHandler(Envelope envelope)
        {
            _envelope = envelope;
        }

        public void Handle(T message)
        {
            Debug.WriteLine("Got message {0}:{1}", typeof(T).Name, message.Id);
            TestMessageRecorder.Processed(GetType().Name, message, _envelope.ReceivedAt);
        }
    }
}