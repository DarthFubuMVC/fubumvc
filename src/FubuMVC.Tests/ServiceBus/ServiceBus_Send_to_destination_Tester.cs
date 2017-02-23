using System;
using System.Linq;
using FubuCore;
using FubuMVC.Core.ServiceBus.Events;
using FubuMVC.Core.ServiceBus.Runtime;
using FubuMVC.Tests.TestSupport;
using Xunit;
using Rhino.Mocks;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus
{
    
    public class ServiceBus_Send_to_destination_Tester : InteractionContext<Core.ServiceBus.ServiceBus>
    {
        private Envelope theLastEnvelopeSent
        {
            get
            {
                return MockFor<IEnvelopeSender>().GetArgumentsForCallsMadeOn(x => x.Send(null))
                    .Last()[0].As<Envelope>();
            }
        }

        [Fact]
        public void sends_to_appropriate_destination()
        {
            var destination = new Uri("memory://blah");
            var message = new Message1();
            
            ClassUnderTest.Send(destination, message);

            theLastEnvelopeSent.Destination.ShouldBe(destination);
            theLastEnvelopeSent.Message.ShouldBeTheSameAs(message);
        }

        [Fact]
        public void sends_to_appropriate_destination_and_waits()
        {
            var destination = new Uri("memory://blah");
            var message = new Message1();
            
            ClassUnderTest.SendAndWait(destination, message).ShouldNotBeNull();

            theLastEnvelopeSent.Destination.ShouldBe(destination);
            theLastEnvelopeSent.Message.ShouldBeTheSameAs(message);

            var lastReplyListener = MockFor<IEventAggregator>().GetArgumentsForCallsMadeOn(x => x.AddListener(null))
                .Last()[0].As<ReplyListener<Acknowledgement>>();
            lastReplyListener.IsExpired.ShouldBeFalse();
            MockFor<IEventAggregator>().AssertWasCalled(x => x.AddListener(Arg<ReplyListener<Acknowledgement>>.Is.Anything));
        }
    }
}