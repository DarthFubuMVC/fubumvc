using System;
using System.Linq;
using FubuCore.Logging;
using FubuMVC.Core;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.Registration.Nodes;
using FubuMVC.Core.ServiceBus.Runtime;
using FubuMVC.Core.ServiceBus.Runtime.Headers;
using FubuMVC.Core.ServiceBus.Runtime.Invocation;
using FubuMVC.Tests.TestSupport;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;
using TestMessages.ScenarioSupport;

namespace FubuMVC.Tests.ServiceBus.Runtime.Invocation
{
    [TestFixture]
    public class find_the_chain_by_envelope_message_type
    {
        [Test]
        public void can_find_chain_for_input_type()
        {
            using (var runtime = FubuRuntime.BasicBus(x =>
            {
                x.Handlers.Include<OneHandler>();
                x.Handlers.Include<TwoHandler>();

                x.Handlers.DisableDefaultHandlerSource();
            }))
            {
                var graph = runtime.Behaviors;


                var invoker = new ChainInvoker(null, graph, null, null, null);

                invoker.FindChain(new Envelope { Message = new OneMessage() })
                       .OfType<HandlerCall>().Single()
                       .HandlerType.ShouldBe(typeof(OneHandler));
            }
        }

        
    }

    [TestFixture]
    public class when_executing_the_chain_happy_path : MessageInvokerContext
    {
        private IDisposableBehavior theBehavior;
        private FubuMVC.Core.ServiceBus.Runtime.Invocation.InvocationContext _invocationContext;

        protected override void theContextIs()
        {
            theBehavior = MockFor<IDisposableBehavior>();
            theChain = new HandlerChain();

            _invocationContext = new FubuMVC.Core.ServiceBus.Runtime.Invocation.InvocationContext(theEnvelope, new HandlerChain());

            MockFor<IServiceFactory>().Stub(x => x.BuildBehavior(_invocationContext, theChain.UniqueId))
                .Return(theBehavior);

            Services.Inject(new BehaviorGraph());

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

            Services.Inject(new BehaviorGraph());

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