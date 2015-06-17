using System.Linq;
using FubuTestingSupport;
using FubuTransportation.ErrorHandling;
using FubuTransportation.Runtime;
using NUnit.Framework;

namespace FubuTransportation.Testing.ErrorHandling
{
    [TestFixture]
    public class RespondWithMessageContinuationTester
    {
        [SetUp]
        public void SetUp()
        {
            _envelope = ObjectMother.Envelope();
            _message = new object();
            _context = new TestContinuationContext();

            new RespondWithMessageContinuation(_message).Execute(_envelope, _context);
        }

        private Envelope _envelope;
        private object _message;
        private TestContinuationContext _context;

        [Test]
        public void should_send_the_message()
        {
            var message = _context.RecordedOutgoing.Outgoing.Single();
            message.ShouldBeTheSameAs(_message);
        }
    }
}