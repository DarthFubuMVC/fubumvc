using System;
using FubuMVC.Core.ServiceBus.Runtime.Invocation;
using Xunit;
using Rhino.Mocks;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus.Runtime.Invocation
{
    
    public class ResponseEnvelopeHandlerTester
    {
        [Fact]
        public void matches_positive()
        {
            var envelope = ObjectMother.Envelope();
            envelope.ResponseId = Guid.NewGuid().ToString();

            new ResponseEnvelopeHandler().Matches(envelope)
                .ShouldBeTrue();
        }

        [Fact]
        public void matches_negative()
        {
            var envelope = ObjectMother.Envelope();
            envelope.ResponseId = null;

            new ResponseEnvelopeHandler().Matches(envelope)
                .ShouldBeFalse();
        }

        [Fact]
        public void handle()
        {
            // pretty simple.  Effectively just shuts things down.
            var context = new TestEnvelopeContext();

                        var envelope = ObjectMother.Envelope();
            envelope.ResponseId = Guid.NewGuid().ToString();

            new ResponseEnvelopeHandler().Execute(envelope, context);

            envelope.Callback.AssertWasCalled(x => x.MarkSuccessful());
        }
    }
}