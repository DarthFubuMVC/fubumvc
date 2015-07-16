using System.Diagnostics;
using FubuMVC.Core.Services.Messaging.Tracking;
using FubuTransportation.Runtime;

namespace FubuTransportation.Testing.ScenarioSupport
{
    public class SourceRecordingHandler
    {
        private readonly Envelope _envelope;

        public SourceRecordingHandler(Envelope envelope)
        {
            _envelope = envelope;
        }

        public void Consume(Message message)
        {
            message.Source = _envelope.ReceivedAt;
            message.Envelope = _envelope;

            MessageHistory.Record(MessageTrack.ForReceived(message, message.Id.ToString()));
        }
    }
}