using FubuTransportation.Runtime.Cascading;

namespace FubuTransportation.Serenity.Samples.SystemUnderTest
{
    public class TestMessageHandler
    {
        private readonly MessageRecorder _messageRecorder;

        public TestMessageHandler(MessageRecorder messageRecorder)
        {
            _messageRecorder = messageRecorder;
        }

        public Respond Handle(TestMessage message)
        {
            _messageRecorder.Messages.Add(message);
            return Respond.With(new TestResponse()).ToSender();
        }
    }
}