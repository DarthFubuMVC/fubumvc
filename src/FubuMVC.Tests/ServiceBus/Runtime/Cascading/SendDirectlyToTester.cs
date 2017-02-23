using System;
using FubuMVC.Core.ServiceBus.Runtime;
using FubuMVC.Core.ServiceBus.Runtime.Cascading;
using Xunit;
using Rhino.Mocks;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus.Runtime.Cascading
{
    
    public class SendDirectlyToTester
    {
        [Fact]
        public void create_and_send_to_correct_destination()
        {
            var destination = new Uri("memory://blah");
            var message = new Message1();
            var sendDirectlyTo = new SendDirectlyTo(destination, message);

            var original = MockRepository.GenerateMock<Envelope>();
            var response = new Envelope();

            original.Stub(x => x.ForSend(message)).Return(response);

            sendDirectlyTo.CreateEnvelope(original).ShouldBeTheSameAs(response);
            response.Destination.ShouldBe(destination);
        }
    }
}