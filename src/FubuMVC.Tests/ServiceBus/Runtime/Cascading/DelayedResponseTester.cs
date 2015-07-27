using FubuCore;
using FubuMVC.Core.ServiceBus.Runtime;
using FubuMVC.Core.ServiceBus.Runtime.Cascading;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus.Runtime.Cascading
{
    [TestFixture]
    public class DelayedResponseTester
    {
        [Test]
        public void create_and_send_with_timespan_delayed()
        {
            var message = new Message1();
            var delayed = new DelayedResponse(message, 5.Minutes());

            var original = MockRepository.GenerateMock<Envelope>();
            var response = new Envelope();

            original.Stub(x => x.ForResponse(message)).Return(response);

            delayed.CreateEnvelope(original).ShouldBeTheSameAs(response);
            response.ExecutionTime.ShouldBe(delayed.Time);
        }
    }
}   