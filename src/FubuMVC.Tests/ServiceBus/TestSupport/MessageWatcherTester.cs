using System;
using System.Linq;
using FubuMVC.Core.ServiceBus.Logging;
using FubuMVC.Core.ServiceBus.Runtime;
using FubuMVC.Core.ServiceBus.TestSupport;
using FubuMVC.Core.Services.Messaging.Tracking;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus.TestSupport
{
    [TestFixture]
    public class MessageWatcherTester
    {
        [Test]
        public void handle_chain_started()
        {
            MessageHistory.StartListening();


            var @event = new ChainExecutionStarted
            {
                ChainId = Guid.NewGuid(), Envelope = new EnvelopeToken()
            };

            new MessageWatcher().Handle(@event);

            var sent = MessageHistory.Outstanding().Single();
            sent.Id.ShouldBe(@event.Envelope.CorrelationId);
            sent.Description.ShouldBe(@event.ToString());
            sent.Type.ShouldBe(MessageWatcher.MessageTrackType);
        }

        [Test]
        public void handle_chain_finished()
        {
            MessageHistory.StartListening();

            var @event = new ChainExecutionStarted
            {
                ChainId = Guid.NewGuid(),
                Envelope = new EnvelopeToken()
            };

            var messageWatcher = new MessageWatcher();
            messageWatcher.Handle(@event);

            var finished = new ChainExecutionFinished
            {
                ChainId = @event.ChainId,
                Envelope = @event.Envelope
            };

            messageWatcher.Handle(finished);

            var received = MessageHistory.Received().Single();
            received.Id.ShouldBe(@event.Envelope.CorrelationId);
            received.Description.ShouldBe(finished.ToString());
            received.Type.ShouldBe(MessageWatcher.MessageTrackType);

            MessageHistory.Outstanding().Any().ShouldBeFalse();
        }

        [Test]
        public void handle_envelope_sent_then_message_successful_tracking_for_the_same_message_to_multiple_nodes()
        {
            MessageHistory.StartListening();

            var envelope1 = new EnvelopeToken();
            var node1 = new StubChannelNode();
            var node2 = new StubChannelNode();

            var messageWatcher = new MessageWatcher();
        
            messageWatcher.Handle(new EnvelopeSent(envelope1, node1));
            messageWatcher.Handle(new EnvelopeSent(envelope1, node2));

            MessageHistory.Outstanding().Count().ShouldBe(2);

            envelope1.Destination = node1.Uri;
            messageWatcher.Handle(new MessageSuccessful
            {
                Envelope = envelope1
            });

            MessageHistory.Outstanding().Count().ShouldBe(1);

            envelope1.Destination = node2.Uri;
            messageWatcher.Handle(new MessageSuccessful
            {
                Envelope = envelope1
            });

            MessageHistory.Outstanding().Any().ShouldBeFalse();
        }

        [Test]
        public void handle_envelope_sent_then_message_successful_for_multiple_messages_to_the_same_node()
        {
            MessageHistory.StartListening();

            var envelope1 = new EnvelopeToken();
            var envelope2 = new EnvelopeToken();
            var node1 = new StubChannelNode();

            var messageWatcher = new MessageWatcher();

            messageWatcher.Handle(new EnvelopeSent(envelope1, node1));
            messageWatcher.Handle(new EnvelopeSent(envelope2, node1));

            MessageHistory.Outstanding().Count().ShouldBe(2);

            envelope1.Destination = node1.Uri;
            messageWatcher.Handle(new MessageSuccessful
            {
                Envelope = envelope1
            });

            MessageHistory.Outstanding().Count().ShouldBe(1);

            envelope2.Destination = node1.Uri;
            messageWatcher.Handle(new MessageSuccessful
            {
                Envelope = envelope2
            });

            MessageHistory.Outstanding().Any().ShouldBeFalse();
        }



        [Test]
        public void handle_envelope_sent_then_message_failed_tracking_for_the_same_message_to_multiple_nodes()
        {
            MessageHistory.StartListening();

            var envelope1 = new EnvelopeToken();
            var node1 = new StubChannelNode();
            var node2 = new StubChannelNode();

            var messageWatcher = new MessageWatcher();

            messageWatcher.Handle(new EnvelopeSent(envelope1, node1));
            messageWatcher.Handle(new EnvelopeSent(envelope1, node2));

            MessageHistory.Outstanding().Count().ShouldBe(2);

            envelope1.Destination = node1.Uri;
            messageWatcher.Handle(new MessageFailed
            {
                Envelope = envelope1
            });

            MessageHistory.Outstanding().Count().ShouldBe(1);

            envelope1.Destination = node2.Uri;
            messageWatcher.Handle(new MessageFailed
            {
                Envelope = envelope1
            });

            MessageHistory.Outstanding().Any().ShouldBeFalse();
        }

        [Test]
        public void handle_envelope_sent_then_message_failed_for_multiple_messages_to_the_same_node()
        {
            MessageHistory.StartListening();

            var envelope1 = new EnvelopeToken();
            var envelope2 = new EnvelopeToken();
            var node1 = new StubChannelNode();

            var messageWatcher = new MessageWatcher();

            messageWatcher.Handle(new EnvelopeSent(envelope1, node1));
            messageWatcher.Handle(new EnvelopeSent(envelope2, node1));

            MessageHistory.Outstanding().Count().ShouldBe(2);

            envelope1.Destination = node1.Uri;
            messageWatcher.Handle(new MessageFailed
            {
                Envelope = envelope1
            });

            MessageHistory.Outstanding().Count().ShouldBe(1);

            envelope2.Destination = node1.Uri;
            messageWatcher.Handle(new MessageFailed
            {
                Envelope = envelope2
            });

            MessageHistory.Outstanding().Any().ShouldBeFalse();
        }


    }
}