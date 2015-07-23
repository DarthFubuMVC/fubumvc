using System;
using System.Linq;
using System.Threading;
using FubuMVC.Core.ServiceBus;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.Events;
using FubuMVC.Core.ServiceBus.Polling;
using FubuMVC.Tests.ServiceBus.InMemory;
using FubuTestingSupport;
using FubuTransportation.Testing.InMemory;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuTransportation.Testing.Events
{
    [TestFixture]
    public class ExpiringListenerCleanupTester : InteractionContext<ExpiringListenerCleanup>
    {
        [Test]
        public void execute_prunes_for_the_current_time()
        {
            LocalSystemTime = DateTime.Today.AddHours(8);

            ClassUnderTest.Execute(new CancellationToken());

            MockFor<IEventAggregator>().AssertWasCalled(x => x.PruneExpiredListeners(UtcSystemTime));
        }
    }

    [TestFixture]
    public class Expiring_listener_polling_job_is_registered
    {
        [Test]
        public void the_cleanup_job_is_registered()
        {
            FubuTransport.SetupForInMemoryTesting();

            using (var runtime = FubuTransport.For<DelayedRegistry>()
                .Bootstrap())
            {
                runtime.Factory.Get<IPollingJobs>()
                    .Any(x => x is PollingJob<ExpiringListenerCleanup, TransportSettings>)
                    .ShouldBeTrue();
            }
        }

        [TearDown]
        public void TearDown()
        {
            FubuTransport.Reset();
        }
    }
}