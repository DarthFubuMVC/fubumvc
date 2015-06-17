using System;
using Bottles.Services.Messaging.Tracking;
using FubuTransportation.Logging;
using FubuTransportation.Runtime;
using FubuTransportation.TestSupport;
using NUnit.Framework;
using System.Linq;
using FubuTestingSupport;

namespace FubuTransportation.Testing.TestSupport
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
            sent.Id.ShouldEqual(@event.Envelope.CorrelationId);
            sent.Description.ShouldEqual(@event.ToString());
            sent.Type.ShouldEqual(MessageWatcher.MessageTrackType);
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
            received.Id.ShouldEqual(@event.Envelope.CorrelationId);
            received.Description.ShouldEqual(finished.ToString());
            received.Type.ShouldEqual(MessageWatcher.MessageTrackType);

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

            MessageHistory.Outstanding().Count().ShouldEqual(2);

            envelope1.Destination = node1.Uri;
            messageWatcher.Handle(new MessageSuccessful
            {
                Envelope = envelope1
            });

            MessageHistory.Outstanding().Count().ShouldEqual(1);

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

            MessageHistory.Outstanding().Count().ShouldEqual(2);

            envelope1.Destination = node1.Uri;
            messageWatcher.Handle(new MessageSuccessful
            {
                Envelope = envelope1
            });

            MessageHistory.Outstanding().Count().ShouldEqual(1);

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

            MessageHistory.Outstanding().Count().ShouldEqual(2);

            envelope1.Destination = node1.Uri;
            messageWatcher.Handle(new MessageFailed
            {
                Envelope = envelope1
            });

            MessageHistory.Outstanding().Count().ShouldEqual(1);

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

            MessageHistory.Outstanding().Count().ShouldEqual(2);

            envelope1.Destination = node1.Uri;
            messageWatcher.Handle(new MessageFailed
            {
                Envelope = envelope1
            });

            MessageHistory.Outstanding().Count().ShouldEqual(1);

            envelope2.Destination = node1.Uri;
            messageWatcher.Handle(new MessageFailed
            {
                Envelope = envelope2
            });

            MessageHistory.Outstanding().Any().ShouldBeFalse();
        }


    }
}