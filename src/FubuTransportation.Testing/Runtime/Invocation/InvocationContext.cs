using System;
using System.Collections.Generic;
using FubuMVC.Core;
using FubuMVC.Core.StructureMap;
using FubuTransportation.Configuration;
using FubuTransportation.Runtime;
using FubuTransportation.Runtime.Invocation;
using FubuTransportation.Runtime.Serializers;
using FubuTransportation.Testing.ScenarioSupport;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap;
using System.Linq;
using FubuCore;

namespace FubuTransportation.Testing.Runtime.Invocation
{
    public abstract class InvocationContext
    {
        private FubuTransportRegistry theTransportRegistry;
        private Lazy<IChainInvoker> _invoker;

        protected IMessageCallback theCallback;

        private FubuRuntime theRuntime;
            
            
            
        [SetUp]
        public void SetUp()
        {
            theTransportRegistry = FubuTransportRegistry.Empty();
            theTransportRegistry.Handlers.DisableDefaultHandlerSource();
            theTransportRegistry.EnableInMemoryTransport();

            TestMessageRecorder.Clear();

            _invoker = new Lazy<IChainInvoker>(() => {
                var container = new Container();
                theRuntime = FubuTransport.For(theTransportRegistry).StructureMap(container).Bootstrap();

                return container.GetInstance<IChainInvoker>();
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