using FubuCore;
using FubuTestingSupport;
using FubuTransportation.Runtime;
using NUnit.Framework;
using Rhino.Mocks;
using Rhino.Mocks.Constraints;

namespace FubuTransportation.Testing.Runtime.Cascading
{
    [TestFixture]
    public class DelayedResponseTester
    {
        [Test]
        public void create_and_send_with_timespan_delayed()
        {
            var message = new Message1();
            var delayed = new FubuTransportation.Runtime.Cascading.DelayedResponse(message, 5.Minutes());

            var original = MockRepository.GenerateMock<Envelope>();
            var response = new Envelope();

            original.Stub(x => x.ForResponse(message)).Return(response);

            delayed.CreateEnvelope(original).ShouldBeTheSameAs(response);
            response.ExecutionTime.ShouldEqual(delayed.Time);
        }
    }
}   