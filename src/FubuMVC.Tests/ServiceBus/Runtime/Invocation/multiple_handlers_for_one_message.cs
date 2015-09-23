using System.Linq;
using NUnit.Framework;
using Shouldly;
using TestMessages.ScenarioSupport;

namespace FubuMVC.Tests.ServiceBus.Runtime.Invocation
{
    [TestFixture]
    public class multiple_handlers_for_one_message : InvocationContext
    {
        [Test]
        public void multiple_handlers_should_fire_for_message()
        {
            handler<OneHandler, DifferentOneHandler, AnotherOneHandler, TwoHandler>();

            var theMessage = new OneMessage();
            sendMessage(theMessage);

            TestMessageRecorder.AllProcessed.Count().ShouldBe(3);
            TestMessageRecorder.AllProcessed.ElementAt(0).ShouldMatch<OneHandler>(theMessage);
            TestMessageRecorder.AllProcessed.ElementAt(1).ShouldMatch<DifferentOneHandler>(theMessage);
            TestMessageRecorder.AllProcessed.ElementAt(2).ShouldMatch<AnotherOneHandler>(theMessage);
        }
    }
}