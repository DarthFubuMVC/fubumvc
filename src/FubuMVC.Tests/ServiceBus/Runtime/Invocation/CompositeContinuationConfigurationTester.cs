using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FubuMVC.Core;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.Runtime.Cascading;
using Xunit;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus.Runtime.Invocation
{
    
    public class CompositeContinuationConfigurationTester
    {
        [Fact]
        public void should_place_composite_error_handler()
        {
            var registry = new FubuRegistry();
            registry.ServiceBus.Enable(true);
            registry.Handlers.Include<MyConsumer>();
            registry.ServiceBus.EnableInMemoryTransport();

            registry.Policies.Global.Add<RespondThenMoveToErrorsPolicy>();

            using (var runtime = registry.ToRuntime())
            {
                runtime.Behaviors.Handlers.Each(x =>
                {
                    x.ErrorHandlers.Count.ShouldBe(1);
                });
            }
        }
    }

    public class RespondThenMoveToErrorsPolicy : HandlerChainPolicy
    {
        public override void Configure(HandlerChain handlerChain)
        {
            handlerChain.MaximumAttempts = 2;
            handlerChain.OnException<UnauthorizedAccessException>()
                .RespondWithMessage((ex, envelope) => new RespondToSender(new ErrorResponse { Message = ex.Message }))
                .Then.MoveToErrorQueue();
        }
    }

    public class ErrorResponse
    {
        public string Message { get; set; }
    }
}