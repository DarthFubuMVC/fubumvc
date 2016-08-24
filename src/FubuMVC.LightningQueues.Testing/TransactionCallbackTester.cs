using System;
using System.Security.Cryptography.X509Certificates;
using FubuMVC.Core.ServiceBus.Runtime;
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
