using FubuTestingSupport;
using FubuTransportation.Configuration;
using FubuTransportation.Registration.Nodes;
using FubuTransportation.Runtime;
using FubuTransportation.Runtime.Invocation;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuTransportation.Testing.Runtime.Invocation
{
    [TestFixture]
    public class sending_a_message_with_no_consumer : InteractionContext<MessageExecutor>
    {
        [Test]
        public void tryexecute_chooses_the_right_path()
        {
            var fakeMessage = new FakeMessage();
            var consumerCalled = false;
            var fakeEnvelope = new Envelope();

            Services.Inject(fakeEnvelope);
            Services.PartialMockTheClassUnderTest();

            ClassUnderTest.TryExecute(fakeMessage, _ => consumerCalled = true);
            Assert.IsTrue(consumerCalled);
            ClassUnderTest.AssertWasNotCalled(x => x.Execute(fakeMessage));
        }

        [Test]
        public void should_call_execute_method()
        {
            var fakeMessage = new FakeMessage();
            var consumerCalled = false;
            var fakeEnvelope = new Envelope();
            var fakeGraph = new HandlerGraph
                {
                    new HandlerCall(typeof (FakeMessage), typeof (FakeHandler).GetMethod("FakeMethod"))
                };

            Services.Inject(fakeGraph);
            Services.Inject(fakeEnvelope);
            Services.PartialMockTheClassUnderTest();
            ClassUnderTest.Expect(x => x.Execute(fakeMessage));

            ClassUnderTest.TryExecute(fakeMessage, _ => consumerCalled = true);
            
            Assert.IsFalse(consumerCalled);
            ClassUnderTest.AssertWasCalled(x => x.Execute(fakeMessage));
        }

        public class FakeMessage
        {
            
        }

        public class FakeHandler
        {
            public void FakeMethod(FakeMessage fake)
            {
                
            }
        }
    }
}