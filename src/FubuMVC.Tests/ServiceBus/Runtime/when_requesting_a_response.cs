using System.Linq;
using System.Threading.Tasks;
using FubuCore;
using FubuMVC.Core.ServiceBus.Events;
using FubuMVC.Core.ServiceBus.Runtime;
using FubuMVC.Tests.TestSupport;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus.Runtime
{
    [TestFixture]
    public class when_requesting_a_response : InteractionContext<Core.ServiceBus.ServiceBus>
    {
        private RecordingEnvelopeSender theSender;
        private Message1 theRequest;
        private Task<Message2> theTask;
        private Envelope theEnvelope;

        protected override void beforeEach()
        {
            theSender = new RecordingEnvelopeSender();
            Services.Inject<IEnvelopeSender>(theSender);

            theRequest = new Message1();
            theTask = ClassUnderTest.Request<Message2>(theRequest);

            theEnvelope = theSender.Sent.Single();
        }

        [Test]
        public void the_envelope_is_sent_with_reply_requested_header()
        {
            theEnvelope.ReplyRequested.ShouldBe(typeof(Message2).Name);
        }

        [Test]
        public void should_register_a_reply_listener()
        {
            var events = MockFor<IEventAggregator>();
            var expectedListener = new ReplyListener<Message2>(events,
                                                               theEnvelope.CorrelationId, 10.Minutes());


            events.AssertWasCalled(x => x.AddListener(expectedListener));
        }

        [Test]
        public void sends_the_envelope_to_the_sender()
        {
            theEnvelope.ShouldNotBeNull();
        }

    }
}