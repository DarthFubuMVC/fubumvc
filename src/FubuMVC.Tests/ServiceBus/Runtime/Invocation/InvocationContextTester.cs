using System.Linq;
using FubuMVC.Core.Http;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.Runtime;
using FubuMVC.Core.ServiceBus.Runtime.Invocation;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus.Runtime.Invocation
{
    [TestFixture]
    public class InvocationContextTester
    {
        [Test]
        public void registers_a_CurrentChain_service_for_diagnostic_purposes()
        {
            var chain = new HandlerChain();
            var context = new FubuMVC.Core.ServiceBus.Runtime.Invocation.InvocationContext(ObjectMother.EnvelopeWithMessage(), chain);

            context.Get<ICurrentChain>().Current.ShouldBeTheSameAs(chain);

        }

        [Test]
        public void enqueue()
        {
            var messages = new FubuMVC.Core.ServiceBus.Runtime.Invocation.InvocationContext(new Envelope{Message = new Message1()}, new HandlerChain());
            var m1 = new Message1();
            var m2 = new Message2();

            messages.EnqueueCascading(m1);
            messages.EnqueueCascading(m2);

            messages.OutgoingMessages().ShouldHaveTheSameElementsAs(m1, m2);
        }

        [Test]
        public void ignores_nulls_just_fine()
        {
            var messages = new FubuMVC.Core.ServiceBus.Runtime.Invocation.InvocationContext(new Envelope { Message = new Message1() }, new HandlerChain());
            messages.EnqueueCascading(null);

            messages.OutgoingMessages().Any().ShouldBeFalse();
        }

        [Test]
        public void enqueue_an_oject_array()
        {
            var messages = new FubuMVC.Core.ServiceBus.Runtime.Invocation.InvocationContext(new Envelope{Message = new Message1()}, new HandlerChain());
            var m1 = new Message1();
            var m2 = new Message2();

            messages.EnqueueCascading(new object[]{m1, m2});

            messages.OutgoingMessages().ShouldHaveTheSameElementsAs(m1, m2);
        }
    }

    [TestFixture]
    public class when_building_a_new_handler_arguments_object
    {
        private Envelope theEnvelope;
        private FubuMVC.Core.ServiceBus.Runtime.Invocation.InvocationContext theArgs;

        [SetUp]
        public void SetUp()
        {
            theEnvelope = new Envelope{Message = new Message2()};

            theArgs = new FubuMVC.Core.ServiceBus.Runtime.Invocation.InvocationContext(theEnvelope, new HandlerChain());
        }

        [Test]
        public void should_set_an_IFubuRequest_with_the_message_set()
        {
            theArgs.Get<IFubuRequest>().Get<Message2>()
                   .ShouldBeTheSameAs(theEnvelope.Message);
        }

        [Test]
        public void registers_itself_as_the_outgoing_messages()
        {
            theArgs.Get<IInvocationContext>().ShouldBeTheSameAs(theArgs);
        }

        [Test]
        public void registers_The_envelope()
        {
            theArgs.Get<Envelope>().ShouldBeTheSameAs(theEnvelope);
        }
    }
}