using System.Linq;
using FubuMVC.Tests.ServiceBus.Runtime.Invocation;
using NUnit.Framework;
using Shouldly;
using TestMessages.ScenarioSupport;

namespace FubuMVC.Tests.ServiceBus.Runtime
{
    [TestFixture]
    public class generic_handlers_are_called_for_messages_that_match : InvocationContext
    {
        [Test]
        public void generic_handler_is_applied_at_end()
        {
            handler<OneHandler, TwoHandler, ThreeHandler, GenericHandler>();

            var message1 = new OneMessage();
            var message2 = new TwoMessage();
            var message3 = new ThreeMessage();

            sendMessage(message1);
            sendMessage(message2);
            sendMessage(message3);

            TestMessageRecorder.AllProcessed.Count().ShouldBe(6);
            TestMessageRecorder.AllProcessed[0].ShouldMatch<OneHandler>(message1);
            TestMessageRecorder.AllProcessed[1].ShouldMatch<GenericHandler>(message1);
            TestMessageRecorder.AllProcessed[2].ShouldMatch<TwoHandler>(message2);
            TestMessageRecorder.AllProcessed[3].ShouldMatch<GenericHandler>(message2);
            TestMessageRecorder.AllProcessed[4].ShouldMatch<ThreeHandler>(message3);
            TestMessageRecorder.AllProcessed[5].ShouldMatch<GenericHandler>(message3);
        }
    }
}