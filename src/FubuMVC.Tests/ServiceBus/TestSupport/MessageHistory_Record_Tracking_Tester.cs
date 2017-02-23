using System;
using System.Linq;
using FubuMVC.Core.ServiceBus.Logging;
using FubuMVC.Core.ServiceBus.Runtime;
using FubuMVC.Core.Services.Messaging.Tracking;
using Xunit;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus.TestSupport
{
    
    public class MessageHistory_Record_Tracking_Tester
    {
        [Fact]
        public void handle_chain_started()
        {
            MessageHistory.ClearHistory();

            var @event = new ChainExecutionStarted
            {
                ChainId = Guid.NewGuid(), Envelope = new EnvelopeToken()
            };


            var sent = @event.ToMessageTrack();
            sent.Id.ShouldBe(@event.Envelope.CorrelationId);
            sent.Description.ShouldBe(@event.ToString());
            sent.Type.ShouldBe(MessageLogRecord.MessageTrackType);
        }

        [Fact]
        public void handle_chain_finished()
        {
            MessageHistory.ClearHistory();

            var @event = new ChainExecutionStarted
            {
                ChainId = Guid.NewGuid(),
                Envelope = new EnvelopeToken()
            };

            var finished = new ChainExecutionFinished
            {
                ChainId = @event.ChainId,
                Envelope = @event.Envelope
            };

            var received = finished.ToMessageTrack();
            received.Id.ShouldBe(@event.Envelope.CorrelationId);
            received.Description.ShouldBe(finished.ToString());
            received.Type.ShouldBe(MessageLogRecord.MessageTrackType);

            MessageHistory.Outstanding().Any().ShouldBeFalse();
        }

        [Fact]
        public void handle_envelope_sent_then_message_successful_tracking_for_the_same_message_to_multiple_nodes()
        {
            MessageHistory.ClearHistory();

            var envelope1 = new EnvelopeToken();
            var node1 = new StubChannelNode();
            var node2 = new StubChannelNode();

            MessageHistory.Record(new EnvelopeSent(envelope1, node1).ToMessageTrack());
            MessageHistory.Record(new EnvelopeSent(envelope1, node2).ToMessageTrack());

            MessageHistory.Outstanding().Count().ShouldBe(2);

            envelope1.Destination = node1.Uri;


            MessageHistory.Record(new MessageSuccessful
            {
                Envelope = envelope1
            }.ToMessageTrack());


            MessageHistory.Outstanding().Count().ShouldBe(1);

            envelope1.Destination = node2.Uri;

            MessageHistory.Record(new MessageSuccessful
            {
                Envelope = envelope1
            }.ToMessageTrack());

            MessageHistory.Outstanding().Any().ShouldBeFalse();
        }

        [Fact]
        public void handle_envelope_sent_then_message_successful_for_multiple_messages_to_the_same_node()
        {
            MessageHistory.ClearHistory();

            var envelope1 = new EnvelopeToken();
            var envelope2 = new EnvelopeToken();
            var node1 = new StubChannelNode();


            MessageHistory.Record(new EnvelopeSent(envelope1, node1).ToMessageTrack());
            MessageHistory.Record(new EnvelopeSent(envelope2, node1).ToMessageTrack());

            MessageHistory.Outstanding().Count().ShouldBe(2);

            envelope1.Destination = node1.Uri;

            MessageHistory.Record(new MessageSuccessful
            {
                Envelope = envelope1
            }.ToMessageTrack());


            MessageHistory.Outstanding().Count().ShouldBe(1);

            envelope2.Destination = node1.Uri;

            MessageHistory.Record(new MessageSuccessful
            {
                Envelope = envelope2
            }.ToMessageTrack());

            MessageHistory.Outstanding().Any().ShouldBeFalse();
        }



        [Fact]
        public void handle_envelope_sent_then_message_failed_tracking_for_the_same_message_to_multiple_nodes()
        {
            MessageHistory.ClearHistory();

            var envelope1 = new EnvelopeToken();
            var node1 = new StubChannelNode();
            var node2 = new StubChannelNode();


            

            MessageHistory.Record(new EnvelopeSent(envelope1, node1));
            MessageHistory.Record(new EnvelopeSent(envelope1, node2));

            MessageHistory.Outstanding().Count().ShouldBe(2);

            envelope1.Destination = node1.Uri;
            MessageHistory.Record(new MessageFailed
            {
                Envelope = envelope1
            });

            MessageHistory.Outstanding().Count().ShouldBe(1);

            envelope1.Destination = node2.Uri;
            MessageHistory.Record(new MessageFailed
            {
                Envelope = envelope1
            });

            MessageHistory.Outstanding().Any().ShouldBeFalse();
        }

        [Fact]
        public void handle_envelope_sent_then_message_failed_for_multiple_messages_to_the_same_node()
        {
            MessageHistory.ClearHistory();

            var envelope1 = new EnvelopeToken();
            var envelope2 = new EnvelopeToken();
            var node1 = new StubChannelNode();

            MessageHistory.Record(new EnvelopeSent(envelope1, node1));
            MessageHistory.Record(new EnvelopeSent(envelope2, node1));

            MessageHistory.Outstanding().Count().ShouldBe(2);

            envelope1.Destination = node1.Uri;
            MessageHistory.Record(new MessageFailed
            {
                Envelope = envelope1
            });

            MessageHistory.Outstanding().Count().ShouldBe(1);

            envelope2.Destination = node1.Uri;
            MessageHistory.Record(new MessageFailed
            {
                Envelope = envelope2
            });

            MessageHistory.Outstanding().Any().ShouldBeFalse();
        }


    }
}