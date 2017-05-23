using System;
using FubuMVC.Core.ServiceBus.Runtime;
using FubuMVC.LightningQueues.Queues;
using Xunit;
using Rhino.Mocks;
using Shouldly;

namespace FubuMVC.LightningQueues.Testing
{
    
    public class TransactionCallbackTester
    {
        [Fact]
        public void sending_on_callback_translates_headers()
        {
            var envelope = new Envelope
            {
                Destination = new Uri("lq.tcp://localhost/nowhere")
            };
            envelope.Headers[LightningQueuesChannel.MaxAttemptsHeader] = "1";
            var context = MockRepository.GenerateStub<IQueueContext>();
            context.Stub(x => x.Send(null)).Callback((OutgoingMessage x) =>
            {
                x.MaxAttempts.ShouldBe(1);
                return true;
            });
            var callback = new TransactionCallback(context, new Message());
            callback.Send(envelope);
        }
    }
}
