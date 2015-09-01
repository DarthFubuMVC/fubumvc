using System.Linq;
using FubuMVC.Core.ServiceBus.ErrorHandling;
using FubuMVC.Core.ServiceBus.Runtime;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus.ErrorHandling
{
    [TestFixture]
    public class RespondWithMessageContinuationTester
    {
        [SetUp]
        public void SetUp()
        {
            _envelope = ObjectMother.Envelope();
            _message = new object();
            _context = new TestEnvelopeContext();

            new RespondWithMessageContinuation(_message).Execute(_envelope, _context);
        }

        private Envelope _envelope;
        private object _message;
        private TestEnvelopeContext _context;

        [Test]
        public void should_send_the_message()
        {
            var message = _context.RecordedOutgoing.Outgoing.Single();
            message.ShouldBeTheSameAs(_message);
        }
    }
}