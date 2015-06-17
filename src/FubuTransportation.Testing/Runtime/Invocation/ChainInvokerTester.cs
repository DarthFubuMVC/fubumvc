using System;
using FubuCore.Dates;
using FubuCore.Logging;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Runtime;
using FubuTransportation.Configuration;
using FubuTransportation.Logging;
using FubuTransportation.Registration.Nodes;
using FubuTransportation.Runtime;
using FubuTransportation.Runtime.Delayed;
using FubuTransportation.Runtime.Headers;
using FubuTransportation.Runtime.Invocation;
using FubuTransportation.Runtime.Serializers;
using FubuTransportation.Testing.ScenarioSupport;
using NUnit.Framework;
using System.Linq;
using FubuTestingSupport;
using Rhino.Mocks;

namespace FubuTransportation.Testing.Runtime.Invocation
{




    [TestFixture]
    public class find_the_chain_by_envelope_message_type
    {
        [Test]
        public void can_find_chain_for_input_type()
        {
            var graph = FubuTransportRegistry.HandlerGraphFor(x => {
                x.Handlers.Include<OneHandler>();
                x.Handlers.Include<TwoHandler>();

                x.Handlers.DisableDefaultHandlerSource();
            });

            var invoker = new ChainInvoker(null, graph, null, null, null, null);

            invoker.FindChain(new Envelope {Message = new OneMessage()})
                   .OfType<HandlerCall>().Single()
                   .HandlerType.ShouldEqual(typeof (OneHandler));
        }

        
    }

    [TestFixture]
    public class when_executing_the_chain_happy_path : MessageInvokerContext
    {
        private IDisposableBehavior theBehavior;
        private FubuTransportation.Runtime.Invocation.InvocationContext _invocationContext;

        protected override void theContextIs()
        {
            theBehavior = MockFor<IDisposableBehavior>();
            theChain = new HandlerChain();

            _invocationContext = new FubuTransportation.Runtime.Invocation.InvocationContext(theEnvelope, new HandlerChain());

            MockFor<IServiceFactory>().Stub(x => x.BuildBehavior(_invocationContext, theChain.UniqueId))
                .Return(theBehavior);



            ClassUnderTest.ExecuteChain(theEnvelope, theChain);
        }

        [Test]
        public void does_run_the_behavior_found_from_the_factory()
        {
            theBehavior.AssertWasCalled(x => x.Invoke());
        }

        [Test]
        public void the_behavior_should_be_disposed_to_dispose_The_inner_nested_container()
        {
            theBehavior.AssertWasCalled(x => x.Dispose());
        }


    }

    public interface IDisposableBehavior : IActionBehavior, IDisposable{}

    
    public abstract class MessageInvokerContext : InteractionContext<ChainInvoker>
    {
        protected RecordingLogger theLogger;
        protected IMessageCallback theCallback;
        private HandlerChain _chain;
        protected byte[] theData;
        protected OneMessage theMessage;
        protected NameValueHeaders theHeaders;
        protected Envelope theEnvelope;


        protected sealed override void beforeEach()
        {
            theData = new byte[] {1, 2, 3, 4};
            theMessage = new OneMessage();
            theCallback = MockFor<IMessageCallback>();
            theHeaders = new NameValueHeaders();

            theLogger = new RecordingLogger();

            theEnvelope = new Envelope(theData, theHeaders, theCallback) {Message = theMessage};
            theEnvelope.Message.ShouldBeTheSameAs(theMessage);

            Services.Inject<ILogger>(theLogger);

            Services.PartialMockTheClassUnderTest();
            theContextIs();
        }

        protected virtual void theContextIs()
        {
            
        }

        protected HandlerChain theChain
        {
            set
            {
                ClassUnderTest.Stub(x => x.FindChain(theEnvelope))
                              .Return(value);

                _chain = value;
            }
            get { return _chain; }
        }

        protected void assertInfoMessageWasLogged(object log)
        {
            theLogger.InfoMessages.ShouldContain(log);
        }

        protected void assertInfoMessageWasNotLogged(object log)
        {
            theLogger.InfoMessages.ShouldNotContain(log);
        }


    }
}