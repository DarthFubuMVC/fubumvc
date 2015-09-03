using FubuMVC.Core.Registration.Querying;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.Registration.Nodes;
using FubuMVC.Core.ServiceBus.Runtime;
using FubuMVC.Core.ServiceBus.Runtime.Invocation;
using FubuMVC.Tests.TestSupport;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus.Runtime.Invocation
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
            consumerCalled.ShouldBeTrue();
            ClassUnderTest.AssertWasNotCalled(x => x.Execute(fakeMessage));
        }

        [Test]
        public void should_call_execute_method()
        {
            var fakeMessage = new FakeMessage();
            var consumerCalled = false;
            var fakeEnvelope = new Envelope();


            var chain = HandlerChain.For<FakeHandler>(x => x.FakeMethod(null));
            MockFor<IChainResolver>().Stub(x => x.FindUniqueByType(typeof (FakeMessage))).Return(chain);

            
            Services.Inject(fakeEnvelope);
            Services.PartialMockTheClassUnderTest();
            ClassUnderTest.Expect(x => x.Execute(fakeMessage));

            ClassUnderTest.TryExecute(fakeMessage, _ => consumerCalled = true);
            
            consumerCalled.ShouldBeFalse();
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