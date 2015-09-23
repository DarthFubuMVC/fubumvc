using System;
using FubuCore.Logging;
using FubuMVC.Core.ServiceBus;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.Runtime;
using FubuMVC.Core.ServiceBus.Subscriptions;
using FubuMVC.Tests.TestSupport;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;
using TestMessages.ScenarioSupport;

namespace FubuMVC.Tests.ServiceBus.Runtime
{
    [TestFixture]
    public class when_sending_with_the_expectation_of_a_response : InteractionContext<EnvelopeSender>
    {
        private StubChannelNode destinationNode;
        private Message theMessage;
        private Envelope theEnvelope;
        private RecordingLogger theLogger;
        private Uri replyUri;

        protected override void beforeEach()
        {
            destinationNode = new StubChannelNode("fake");

            replyUri = "fake://foo".ToUri();

            theMessage = new Message();
            theEnvelope = new Envelope { Message = theMessage, ReplyRequested = theMessage.GetType().Name};

            theLogger = new RecordingLogger();
            Services.Inject<ILogger>(theLogger);

            MockFor<ISubscriptionCache>().Stub(x => x.FindDestinationChannels(theEnvelope))
                                     .Return(new ChannelNode[] { destinationNode });

            MockFor<ISubscriptionCache>().Stub(x => x.ReplyUriFor(destinationNode)).Return(replyUri);

            ClassUnderTest.Send(theEnvelope);
        }

        [Test]
        public void should_have_associated_the_reply_channel_with_the_envelope()
        {
            theEnvelope.ReplyUri.ShouldBe(replyUri);
        }
    }
}