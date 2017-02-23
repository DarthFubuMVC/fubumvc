using System;
using FubuMVC.Core.ServiceBus.Diagnostics;
using FubuMVC.Core.ServiceBus.Runtime;
using Xunit;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus.Diagnostics
{
    
    public class when_creating_a_message_record_from_envelope_token
    {
        private EnvelopeToken theToken;
        private MessageRecord theRecord;

        public when_creating_a_message_record_from_envelope_token()
        {
            theToken = ObjectMother.EnvelopeWithMessage().ToToken();
            theToken.ParentId = Guid.NewGuid().ToString();
            theToken.Headers["A"] = "1";
            theToken.Headers["B"] = "2";

            theRecord = new MessageRecord(theToken);
        }

        [Fact]
        public void capture_the_correlation_id()
        {
            theRecord.Id.ShouldBe(theToken.CorrelationId);
        }

        [Fact]
        public void capture_the_parent_id()
        {
            theRecord.ParentId.ShouldBe(theToken.ParentId);
        }

        [Fact]
        public void capture_the_message_type()
        {
            theRecord.Type.ShouldBe(theToken.Message.GetType().FullName);
        }

        [Fact]
        public void capture_the_headers()
        {
            theRecord.Headers.ShouldContain("A=1");
            theRecord.Headers.ShouldContain("B=2");
        }
    }
}