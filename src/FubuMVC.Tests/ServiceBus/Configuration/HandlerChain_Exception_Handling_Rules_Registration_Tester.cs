using System;
using System.Linq;
using FubuCore;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.ErrorHandling;
using FubuMVC.Core.ServiceBus.Runtime;
using FubuMVC.Core.ServiceBus.Runtime.Invocation;
using Xunit;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus.Configuration
{
    
    public class HandlerChain_Exception_Handling_Rules_Registration_Tester
    {
        [Fact]
        public void retry_now()
        {
            var chain = new HandlerChain();

            chain.OnException<NotImplementedException>()
                .Retry();

            var handler = chain.ErrorHandlers.Single().ShouldBeOfType<ErrorHandler>();
            handler.Conditions.Single().ShouldBeOfType<ExceptionTypeMatch<NotImplementedException>>();
            handler.Continuation(null, null).ShouldBeOfType<RetryNowContinuation>();
        }

        [Fact]
        public void requeue()
        {
            var chain = new HandlerChain();

            chain.OnException<NotSupportedException>()
                .Requeue();

            var handler = chain.ErrorHandlers.Single().ShouldBeOfType<ErrorHandler>();
            handler.Conditions.Single().ShouldBeOfType<ExceptionTypeMatch<NotSupportedException>>();
            handler.Continuation(null, null).ShouldBeOfType<RequeueContinuation>();
        }

        [Fact]
        public void move_to_error_queue()
        {
            var chain = new HandlerChain();

            chain.OnException<NotSupportedException>()
                .MoveToErrorQueue();

            chain.ErrorHandlers.Single().As<ErrorHandler>().Sources.Single().ShouldBeOfType<MoveToErrorQueueHandler<NotSupportedException>>();
        }

        [Fact]
        public void retry_later()
        {
            var chain = new HandlerChain();

            chain.OnException<NotSupportedException>()
                .RetryLater(10.Minutes());

            var handler = chain.ErrorHandlers.Single().ShouldBeOfType<ErrorHandler>();
            handler.Conditions.Single().ShouldBeOfType<ExceptionTypeMatch<NotSupportedException>>();
            handler.Continuation(null, null).ShouldBeOfType<DelayedRetryContinuation>()
                .Delay.ShouldBe(10.Minutes());
        }

        [Fact]
        public void add_multiple_continuations()
        {
            var chain = new HandlerChain();

            chain.OnException<NotSupportedException>()
                .RetryLater(10.Minutes())
                .Then
                .ContinueWith<TellTheSenderHeSentSomethingWrong>();

            var handler = chain.ErrorHandlers.Single().ShouldBeOfType<ErrorHandler>();
            var continuation = handler.Continuation(null, null).ShouldBeOfType<CompositeContinuation>();
            continuation.Select(x => x.GetType())
                .ShouldHaveTheSameElementsAs(typeof(DelayedRetryContinuation), typeof(TellTheSenderHeSentSomethingWrong));

        }

        [Fact]
        public void respond_with_message()
        {
            var chain = new HandlerChain();

            chain.OnException<NotImplementedException>()
                .RespondWithMessage((ex, env) => new object());

            chain.ErrorHandlers.Single().As<ErrorHandler>().Sources.Single().ShouldBeOfType<RespondWithMessageHandler<NotImplementedException>>();
        }
    }

    public class TellTheSenderHeSentSomethingWrong : IContinuation
    {
        public void Execute(Envelope envelope, IEnvelopeContext context)
        {
            throw new NotImplementedException();
        }
    }
}
