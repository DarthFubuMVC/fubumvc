using FubuMVC.Tests.ServiceBus.ScenarioSupport;
using NUnit.Framework;
using System.Linq;
using FubuTestingSupport;

namespace FubuTransportation.Testing.Runtime.Invocation
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