using System;
using FubuCore.Descriptions;
using FubuMVC.Core.ServiceBus.ErrorHandling;
using Xunit;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus.ErrorHandling
{
    
    public class RespondWithMessageHandlerTester
    {
        [Fact]
        public void describes_itself()
        {
            var handler = new RespondWithMessageHandler<Exception>(null);
            var description = new Description();
            handler.Describe(description);
            description.Title.ShouldNotBeNull();
        }

        [Fact]
        public void returns_no_continuation_when_exception_does_not_match()
        {
            var handler = new RespondWithMessageHandler<NotImplementedException>(null);
            handler.DetermineContinuation(null, new Exception()).ShouldBeNull();
            handler.DetermineContinuation(null, new SystemException()).ShouldBeNull();
            handler.DetermineContinuation(null, new NullReferenceException()).ShouldBeNull();
        }

        [Fact]
        public void responds_with_message_when_the_exception_matches()
        {
            var message = new object();
            var handler = new RespondWithMessageHandler<Exception>((ex, env) => message);
            handler.DetermineContinuation(null, new Exception())
                .ShouldBeOfType<RespondWithMessageContinuation>()
                .Message.ShouldBeTheSameAs(message);
        }
    }
}