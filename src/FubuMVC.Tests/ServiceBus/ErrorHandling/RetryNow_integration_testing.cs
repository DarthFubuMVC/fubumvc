using System.Data;
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
    public class RetryNow_integration_testing
    {
        [Test]
        public void successfully_retries_now()
        {
            MessageThatBombsHandler.Throws = 2;
            MessageThatBombsHandler.Attempts = 0;
            MessageThatBombsHandler.Successful = null;

            using (var runtime = FubuRuntime.For<RetryNoOnDbConcurrencyRegistry>()
                        
                        )
            {
                var pipeline = runtime.Get<IHandlerPipeline>();
                pipeline.InvokeNow(new Envelope {Message = new MessageThatBombs(), Callback = MockRepository.GenerateMock<IMessageCallback>()});
            }

            MessageThatBombsHandler.Successful.ShouldNotBeNull();
            MessageThatBombsHandler.Attempts.ShouldBeGreaterThan(1);
        }
    }

    public class RetryNoOnDbConcurrencyRegistry : FubuRegistry
    {
        public RetryNoOnDbConcurrencyRegistry()
        {
            ServiceBus.Enable(true);
            ServiceBus.EnableInMemoryTransport();
            Policies.Local.Add<RetryNowOnDbConcurrencyException>();
        }
    }


    public class RetryNowOnDbConcurrencyException : HandlerChainPolicy
    {
        public override void Configure(HandlerChain handlerChain)
        {
            handlerChain.MaximumAttempts = 5;
            handlerChain.OnException<DBConcurrencyException>()
                .Retry();
        }
    }

    public class MessageThatBombs
    {
    }

    public class MessageThatBombsHandler
    {
        public static int Throws = 3;
        public static int Attempts = 0;
        public static MessageThatBombs Successful;

        public void Consume(MessageThatBombs message)
        {
            Attempts++;

            if (Attempts <= Throws)
            {
                throw new DBConcurrencyException();
            }

            Successful = message;
        }
    }
}