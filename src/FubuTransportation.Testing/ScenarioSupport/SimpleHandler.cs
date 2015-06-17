using System.Runtime.InteropServices;
using FubuTransportation.Runtime;

namespace FubuTransportation.Testing.ScenarioSupport
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
            TestMessageRecorder.Processed(GetType().Name, message, _envelope.ReceivedAt);
        }
    }
}