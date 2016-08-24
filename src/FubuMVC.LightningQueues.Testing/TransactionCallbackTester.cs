using LightningQueues;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;

namespace FubuMVC.LightningQueues.Testing
{
    [TestFixture]
    public class TransactionCallbackTester
    {
        [Test]
        public void sending_on_callback_translates_headers()
        {
            var message = new OutgoingMessage();
            message.Headers.Add(LightningQueuesChannel.MaxAttemptsHeader, "1");
            var context = MockRepository.GenerateStub<IQueueContext>();
            var callback = new TransactionCallback(context, message);
            callback.Send(message.ToEnvelope());
            message.MaxAttempts.ShouldBe(1);
        }
    }
}
