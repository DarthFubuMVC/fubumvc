using System;
using FubuTestingSupport;
using LightningQueues;
using NUnit.Framework;

namespace FubuTransportation.LightningQueues.Testing
{
    [TestFixture]
    public class HeaderTranslationTester
    {
        [Test]
        public void translates_max_attempts()
        {
            var message = new MessagePayload();
            message.Headers.Add(LightningQueuesChannel.MaxAttemptsHeader, 1.ToString());
            message.TranslateHeaders();
            message.MaxAttempts.ShouldEqual(1);
        }

        [Test]
        public void translates_deliver_by()
        {
            var now = DateTime.Now;
            var message = new MessagePayload();
            message.Headers.Add(LightningQueuesChannel.DeliverByHeader, now.ToString("o"));
            message.TranslateHeaders();
            message.DeliverBy.ShouldEqual(now);
        }

        [Test]
        public void empty_when_headers_arent_present()
        {
            var message = new MessagePayload();
            message.TranslateHeaders();
            message.MaxAttempts.ShouldBeNull();
            message.DeliverBy.ShouldBeNull();
        }
    }
}