using System;
using FubuMVC.Core;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.Runtime;
using FubuMVC.Core.ServiceBus.Runtime.Invocation;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus.ErrorHandling
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

            using (var runtime = FubuRuntime.For<CountAndRetryOnExceptionRegistry>()
                        
                        )
            {
                var pipeline = runtime.Get<IHandlerPipeline>();
                pipeline.InvokeNow(new Envelope {Message = new MessageThatThrows(), Callback = MockRepository.GenerateMock<IMessageCallback>()});
            }

            CounterContinuation.Counter.ShouldBe(2);
            MessageThatThrowsHandler.Attempts.ShouldBe(3);
            MessageThatThrowsHandler.Successful.ShouldNotBeNull();
        }
    }

    public class CountAndRetryOnExceptionRegistry : FubuRegistry
    {
        public CountAndRetryOnExceptionRegistry()
        {
            ServiceBus.Enable(true);
            ServiceBus.EnableInMemoryTransport();
            Policies.Local.Add<CountAndRetryOnExceptionPolicy>();
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

        public void Execute(Envelope envelope, IEnvelopeContext context)
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