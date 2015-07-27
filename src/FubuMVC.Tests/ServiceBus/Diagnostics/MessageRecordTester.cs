using System;
using FubuMVC.Core.ServiceBus.Diagnostics;
using FubuMVC.Core.ServiceBus.Runtime;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus.Diagnostics
{
    [TestFixture]
    public class when_creating_a_message_record_from_envelope_token
    {
        private EnvelopeToken theToken;
        private MessageRecord theRecord;

        [SetUp]
        public void SetUp()
        {
            theToken = ObjectMother.EnvelopeWithMessage().ToToken();
            theToken.ParentId = Guid.NewGuid().ToString();
            theToken.Headers["A"] = "1";
            theToken.Headers["B"] = "2";

            theRecord = new MessageRecord(theToken);
        }

        [Test]
        public void capture_the_correlation_id()
        {
            theRecord.Id.ShouldBe(theToken.CorrelationId);
        }

        [Test]
        public void capture_the_parent_id()
        {
            theRecord.ParentId.ShouldBe(theToken.ParentId);
        }

        [Test]
        public void capture_the_message_type()
        {
            theRecord.Type.ShouldBe(theToken.Message.GetType().FullName);
        }

        [Test]
        public void capture_the_headers()
        {
            theRecord.Headers.ShouldContain("A=1");
            theRecord.Headers.ShouldContain("B=2");
        }
    }
}