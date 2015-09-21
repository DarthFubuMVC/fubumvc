using FubuMVC.Core.ServiceBus;
using FubuMVC.Core.ServiceBus.Polling;
using FubuMVC.Tests.ServiceBus.ScheduledJobs;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus.Polling
{
    [TestFixture]
    public class PollingJobChainTester
    {
        [Test]
        public void chain_is_definitely_a_polling_job()
        {
            PollingJobChain.For<AJob, TransportSettings>(x => x.DelayMessagePolling)
                .IsPollingJob().ShouldBeTrue();
        }
    }
}