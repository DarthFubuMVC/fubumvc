using FubuMVC.Core.ServiceBus.Diagnostics;
using FubuMVC.Core.ServiceBus.Logging;
using Shouldly;
using NUnit.Framework;

namespace FubuTransportation.Testing.Diagnostics
{
    [TestFixture]
    public class MessageRecordListenerTester
    {
        [Test]
        public void matches_on_message_record_types()
        {
            var listener = new MessageRecordListener(null);
            listener.ListensFor(typeof(EnvelopeSent)).ShouldBeTrue();
            listener.ListensFor(typeof(EnvelopeReceived)).ShouldBeTrue();
            listener.ListensFor(typeof(ChainExecutionStarted)).ShouldBeTrue();
            listener.ListensFor(typeof(ChainExecutionFinished)).ShouldBeTrue();
        }
    }
}