using System;
using System.Linq;
using FubuMVC.Core;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.Runtime;
using FubuMVC.Core.ServiceBus.Runtime.Invocation;
using NUnit.Framework;
using Rhino.Mocks;
using TestMessages.ScenarioSupport;

namespace FubuMVC.Tests.ServiceBus.Runtime.Invocation
{
    public abstract class InvocationContext
    {
        private FubuRegistry theTransportRegistry;
        private Lazy<IChainInvoker> _invoker;

        protected IMessageCallback theCallback;

        private FubuRuntime theRuntime;
            
            
            
        [SetUp]
        public void SetUp()
        {
            theTransportRegistry = new FubuRegistry();
            theTransportRegistry.ServiceBus.Enable(true);

            theTransportRegistry.Handlers.DisableDefaultHandlerSource();
            theTransportRegistry.ServiceBus.EnableInMemoryTransport();

            TestMessageRecorder.Clear();

            _invoker = new Lazy<IChainInvoker>(() =>
            {
                theRuntime = theTransportRegistry.ToRuntime();

                return theRuntime.Get<IChainInvoker>();
            });

            theCallback = MockRepository.GenerateMock<IMessageCallback>();

            theContextIs();

        }

        [TearDown]
        public void Teardown()
        {
            theRuntime.Dispose();
        }

        protected virtual void theContextIs()
        {
            
        }


        protected void handler<T>()
        {
            handlersAre(typeof(T));
        }

        protected void handler<T, T1, T2, T3>()
        {
            handlersAre(typeof(T), typeof(T1), typeof(T2), typeof(T3));
        }


        protected void handlersAre(params Type[] handlers)
        {
            theTransportRegistry.Handlers.Include(handlers);
        }

        protected Envelope sendMessage(params Message[] message)
        {
            return message.Select(o => sendOneMessage(o)).FirstOrDefault();
        }

        protected Envelope sendOneMessage(object message)
        {
            var envelope = new Envelope();
            envelope.Callback = theCallback;
            envelope.Message = message;

            sendEnvelope(envelope);

            return envelope;
        }


        protected void sendEnvelope(Envelope envelope)
        {
            _invoker.Value.Invoke(envelope);
        }

        
    }
}