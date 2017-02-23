using System;
using System.Linq;
using FubuCore;
using FubuMVC.Core.ServiceBus.Events;
using FubuMVC.Core.ServiceBus.Logging;
using FubuMVC.Core.ServiceBus.Runtime;
using Xunit;
using Rhino.Mocks;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus.Runtime
{
    
    public class ReplayListener_expiration_logic_Tester
    {
        [Fact]
        public void uses_the_expiration_time()
        {
            var listener = new ReplyListener<Message1>(null, Guid.NewGuid().ToString(), 10.Minutes());

            listener.IsExpired.ShouldBeFalse();

            listener.ExpiresAt.HasValue.ShouldBeTrue();
            (listener.ExpiresAt > DateTime.UtcNow.AddMinutes(9)).ShouldBeTrue();
            (listener.ExpiresAt < DateTime.UtcNow.AddMinutes(11)).ShouldBeTrue();
        }
    }

    
    public class when_receiving_a_matching_failure_ack
    {
        private IEventAggregator theEvents;
        public readonly string correlationId = Guid.NewGuid().ToString();
        private ReplyListener<Message1> theListener;

        public when_receiving_a_matching_failure_ack()
        {
            theEvents = MockRepository.GenerateMock<IEventAggregator>();

            theListener = new ReplyListener<Message1>(theEvents, correlationId, 10.Minutes());

            var envelope = new EnvelopeToken
            {
                Message = new FailureAcknowledgement
                {
                    CorrelationId = correlationId,
                    Message = "No soup for you!"
                }
            };

            envelope.Headers[Envelope.ResponseIdKey] = correlationId;

            theListener.Handle(new EnvelopeReceived
            {
                Envelope = envelope
            });
        }

        [Fact]
        public void the_listener_should_set_a_failure_on_the_task()
        {
            theListener.Completion.Exception
                .Flatten()
                .InnerExceptions.Single()
                .ShouldBeOfType<ReplyFailureException>()
                .Message.ShouldBe("No soup for you!");
        }

        [Fact]
        public void should_remove_itself_from_the_event_aggregator()
        {
            theEvents.AssertWasCalled(x => x.RemoveListener(theListener));
        }

        [Fact]
        public void should_be_expired()
        {
            theListener.IsExpired.ShouldBeTrue();
        }
    }

    
    public class when_receiving_a_failure_ack_that_does_not_match
    {
        private IEventAggregator theEvents;
        public readonly string correlationId = Guid.NewGuid().ToString();
        private ReplyListener<Message1> theListener;

        public when_receiving_a_failure_ack_that_does_not_match()
        {
            theEvents = MockRepository.GenerateMock<IEventAggregator>();

            theListener = new ReplyListener<Message1>(theEvents, correlationId, 10.Minutes());

            var envelope = new EnvelopeToken
            {
                Message = new FailureAcknowledgement
                {
                    CorrelationId = Guid.NewGuid().ToString(),
                    Message = "No soup for you!"
                }
            };

            envelope.Headers[Envelope.ResponseIdKey] = correlationId;

            theListener.Handle(new EnvelopeReceived
            {
                Envelope = envelope
            });
        }

        [Fact]
        public void the_listener_should_not_be_expired()
        {
            theListener.IsExpired.ShouldBeFalse();
        }

        [Fact]
        public void the_listener_should_not_complete_the_task_in_any_way()
        {
            theListener.Completion.IsCompleted.ShouldBeFalse();
            theListener.Completion.IsFaulted.ShouldBeFalse();
        }

        [Fact]
        public void should_NOT_remove_itself_from_the_event_aggregator()
        {
            theEvents.AssertWasNotCalled(x => x.RemoveListener(theListener));
        }
    }


    
    public class when_receiving_the_matching_reply
    {
        private IEventAggregator theEvents;
        public readonly string correlationId = Guid.NewGuid().ToString();
        private ReplyListener<Message1> theListener;
        private Message1 theMessage;

        public when_receiving_the_matching_reply()
        {
            theEvents = MockRepository.GenerateMock<IEventAggregator>();

            theListener = new ReplyListener<Message1>(theEvents, correlationId, 10.Minutes());

            theMessage = new Message1();
            
            var envelope = new EnvelopeToken
            {
                Message = theMessage
            };

            envelope.Headers[Envelope.ResponseIdKey] = correlationId;

            theListener.Handle(new EnvelopeReceived
            {
                Envelope = envelope
            });
        }

        [Fact]
        public void should_set_the_completion_value()
        {
            theListener.Completion.Result.ShouldBeTheSameAs(theMessage);
        }

        [Fact]
        public void should_remove_itself_from_the_event_aggregator()
        {
            theEvents.AssertWasCalled(x => x.RemoveListener(theListener));
        }
    }

    
    public class ReplyListenerMatchesTester
    {
        private IEventAggregator theEvents;
        public readonly string correlationId = Guid.NewGuid().ToString();
        private ReplyListener<Message1> theListener;

        public ReplyListenerMatchesTester()
        {
            theEvents = MockRepository.GenerateMock<IEventAggregator>();
            theListener = new ReplyListener<Message1>(theEvents, correlationId, 10.Minutes());
        }

        [Fact]
        public void matches_if_type_is_right_and_correlation_id_matches()
        {
            theListener.Matches(new EnvelopeToken
            {
                ResponseId = correlationId,
                Message = new Message1()
            }).ShouldBeTrue();
        }

        [Fact]
        public void does_not_match_if_correlation_id_is_wrong()
        {
            theListener.Matches(new EnvelopeToken
            {
                ResponseId = Guid.NewGuid().ToString(),
                Message = new Message1()
            }).ShouldBeFalse();
        }

        [Fact]
        public void does_not_match_if_the_message_type_is_wrong()
        {
            theListener.Matches(new EnvelopeToken
            {
                ResponseId = correlationId,
                Message = new Message2()
            }).ShouldBeFalse();
        }
    }
}