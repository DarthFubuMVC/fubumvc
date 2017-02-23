using System.Linq;
using FubuMVC.Core.ServiceBus.ErrorHandling;
using FubuMVC.Core.ServiceBus.Runtime;
using Xunit;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus.ErrorHandling
{
    
    public class RespondWithMessageContinuationTester
    {
        public RespondWithMessageContinuationTester()
        {
            _envelope = ObjectMother.Envelope();
            _message = new object();
            _context = new TestEnvelopeContext();

            new RespondWithMessageContinuation(_message).Execute(_envelope, _context);
        }

        private Envelope _envelope;
        private object _message;
        private TestEnvelopeContext _context;

        [Fact]
        public void should_send_the_message()
        {
            var message = _context.RecordedOutgoing.Outgoing.Single();
            message.ShouldBeTheSameAs(_message);
        }
    }
}