using FubuMVC.Core.ServiceBus.Diagnostics;
using FubuMVC.Core.ServiceBus.Logging;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus.Diagnostics
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