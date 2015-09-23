using System;
using System.Collections.Generic;
using FubuCore.Logging;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.Logging;
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
    public class when_sending_a_message : InteractionContext<EnvelopeSender>
    {
        private StubChannelNode node1;
        private StubChannelNode node2;
        private StubChannelNode node3;
        private Message theMessage;
        private Envelope theEnvelope;
        private string correlationId;
        private RecordingLogger theLogger;
        private IEnvelopeModifier[] modifiers;

        protected override void beforeEach()
        {
            node1 = new StubChannelNode();
            node2 = new StubChannelNode();
            node3 = new StubChannelNode();

            theMessage = new Message();
            theEnvelope = new Envelope {Message = theMessage};

            theLogger = new RecordingLogger();
            Services.Inject<ILogger>(theLogger);

            modifiers = Services.CreateMockArrayFor<IEnvelopeModifier>(5);

            MockFor<ISubscriptionCache>().Stub(x => x.FindDestinationChannels(theEnvelope))
                                     .Return(new ChannelNode[] { node1, node2, node3 });

            correlationId = ClassUnderTest.Send(theEnvelope);

        }

        [Test]
        public void adds_the_message_type_header()
        {
            theEnvelope.Headers[Envelope.MessageTypeKey].ShouldBe(theMessage.GetType().FullName);
        }

        [Test]
        public void calls_all_the_modifiers_to_optionally_enhance_the_envelope()
        {
            modifiers.Each(x => x.AssertWasCalled(o => o.Modify(theEnvelope)));
        }

        [Test]
        public void should_audit_each_node_sender_for_the_envelope()
        {
            theLogger.InfoMessages.ShouldContain(new EnvelopeSent(theEnvelope.ToToken(), node1));
            theLogger.InfoMessages.ShouldContain(new EnvelopeSent(theEnvelope.ToToken(), node2));
            theLogger.InfoMessages.ShouldContain(new EnvelopeSent(theEnvelope.ToToken(), node3));
        }

        [Test]
        public void all_nodes_receive_teh_message()
        {
            node1.LastEnvelope.ShouldBeTheSameAs(theEnvelope);
            node2.LastEnvelope.ShouldBeTheSameAs(theEnvelope);
            node3.LastEnvelope.ShouldBeTheSameAs(theEnvelope);
        }

        [Test]
        public void adds_a_correlation_id_to_the_envelope()
        {
            Guid.Parse(theEnvelope.CorrelationId)
                .ShouldNotBe(Guid.Empty);

            theEnvelope.CorrelationId.ShouldBe(correlationId);
        }
    }
}