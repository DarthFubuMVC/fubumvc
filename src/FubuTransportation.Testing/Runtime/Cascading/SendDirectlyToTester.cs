using System;
using FubuTestingSupport;
using FubuTransportation.Runtime;
using FubuTransportation.Runtime.Cascading;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuTransportation.Testing.Runtime.Cascading
{
    [TestFixture]
    public class SendDirectlyToTester
    {
        [Test]
        public void create_and_send_to_correct_destination()
        {
            var destination = new Uri("memory://blah");
            var message = new Message1();
            var sendDirectlyTo = new SendDirectlyTo(destination, message);

            var original = MockRepository.GenerateMock<Envelope>();
            var response = new Envelope();

            original.Stub(x => x.ForSend(message)).Return(response);

            sendDirectlyTo.CreateEnvelope(original).ShouldBeTheSameAs(response);
            response.Destination.ShouldEqual(destination);
        }
    }
}