using System;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.Runtime;
using FubuMVC.Core.ServiceBus.Runtime.Invocation;
using FubuMVC.Core.StructureMap;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap;

namespace FubuTransportation.Testing.ErrorHandling
{
    [TestFixture]
    public class CompositeContinuation_integration_testing
    {
        [Test]
        public void successfully_does_composite_continuations()
        {
            MessageThatThrowsHandler.ThrowsUntilAttemptNumber = 3;
            MessageThatThrowsHandler.Attempts = 0;
            MessageThatThrowsHandler.Successful = null;
            CounterContinuation.Counter = 0;

            using (var runtime = FubuTransport.For<CountAndRetryOnExceptionRegistry>()
                        
                        .Bootstrap())
            {
                var pipeline = runtime.Factory.Get<IHandlerPipeline>();
                pipeline.Invoke(new Envelope {Message = new MessageThatThrows(), Callback = MockRepository.GenerateMock<IMessageCallback>()});
            }

            CounterContinuation.Counter.ShouldEqual(2);
            MessageThatThrowsHandler.Attempts.ShouldEqual(3);
            MessageThatThrowsHandler.Successful.ShouldNotBeNull();
        }
    }

    public class CountAndRetryOnExceptionRegistry : FubuTransportRegistry
    {
        public CountAndRetryOnExceptionRegistry()
        {
            EnableInMemoryTransport();
            Local.Policy<CountAndRetryOnExceptionPolicy>();
        }
    }

    public class CountAndRetryOnExceptionPolicy : HandlerChainPolicy
    {
        public override void Configure(HandlerChain handlerChain)
        {
            handlerChain.MaximumAttempts = 5;
            handlerChain.OnException<UnauthorizedAccessException>()
                .ContinueWith<CounterContinuation>()
                .Then.Retry();
        }
    }

    public class CounterContinuation : IContinuation
    {
        public static int Counter = 0;

        public void Execute(Envelope envelope, ContinuationContext context)
        {
            ++Counter;
        }
    }

    public class MessageThatThrows
    {
    }

    public class MessageThatThrowsHandler
    {
        public static int ThrowsUntilAttemptNumber = 1;
        public static int Attempts = 0;
        public static MessageThatThrows Successful;

        public void Consume(MessageThatThrows message)
        {
            ++Attempts;

            if (Attempts < ThrowsUntilAttemptNumber)
            {
                throw new UnauthorizedAccessException();
            }

            Successful = message;
        }
    }
}