using System;
using FubuMVC.Core.ServiceBus.Runtime;
using FubuMVC.Core.ServiceBus.Runtime.Cascading;
using Xunit;
using Rhino.Mocks;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus.Runtime.Cascading
{
    
    public class RespondToSenderTester
    {
        [Fact]
        public void has_to_set_the_destination_header()
        {
            var message = new Message1();
            var respondToSender = new RespondToSender(message);

            var original = MockRepository.GenerateMock<Envelope>();
            original.ReplyUri = new Uri("memory://me/reply");

            var response = new Envelope();


            original.Stub(x => x.ForResponse(message)).Return(response);


            respondToSender.CreateEnvelope(original).ShouldBeTheSameAs(response);
            response.Destination.ShouldBe(original.ReplyUri);
        }
    }
}