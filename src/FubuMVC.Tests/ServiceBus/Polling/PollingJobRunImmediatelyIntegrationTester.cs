using System.Threading;
using FubuMVC.Core;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.Polling;
using FubuMVC.Tests.TestSupport;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus.Polling
{
    [TestFixture]
    public class PollingJobRunImmediatelyIntegrationTester
    {
        private FubuRuntime theRuntime;

        [TestFixtureSetUp]
        public void SetUp()
        {
            ImmediateJob.Executed = DelayJob.Executed = DisabledJob.Executed = 0;

            theRuntime = FubuRuntime.For<PollingImmediateRegistry>()
                                      
                                      ;
        }

        [TestFixtureTearDown]
        public void Teardown()
        {
            theRuntime.Dispose();
        }

        [Test]
        public void should_only_execute_ImmediateJob_now_and_interval_should_still_work()
        {
            DelayJob.Executed.ShouldBe(0);
            ImmediateJob.Executed.ShouldBe(1);

            Wait.Until(() => ImmediateJob.Executed > 1, timeoutInMilliseconds: 6000);
            ImmediateJob.Executed.ShouldBeGreaterThan(1);
        }

        [Test]
        public void disabled_jobs_are_not_executed_or_started()
        {
            DisabledJob.Executed.ShouldBe(0);
            theRuntime.Get<IPollingJobs>().IsActive<DisabledJob>()
                .ShouldBeFalse();

        }
    }

    public class PollingImmediateRegistry : FubuRegistry
    {
        public PollingImmediateRegistry()
        {
            ServiceBus.Enable(true);

            ServiceBus.EnableInMemoryTransport();

            Polling.RunJob<ImmediateJob>().ScheduledAtInterval<PollingImmediateSettings>(x => x.ImmediateInterval).RunImmediately();
            Polling.RunJob<DelayJob>().ScheduledAtInterval<PollingImmediateSettings>(x => x.DelayInterval);
        }
    }

    public class PollingImmediateSettings
    {
        public PollingImmediateSettings()
        {
            // Make these sufficiently high to allow bootstrapping to finish before any jobs hit their timer
            ImmediateInterval = 5000;
            DelayInterval = 5000;
        }

        public double ImmediateInterval { get; set; }
        public double DelayInterval { get; set; }
    }



    public class ImmediateJob : IJob
    {
        public static int Executed = 0;

        public void Execute(CancellationToken token)
        {
            Executed++;
        }
    }

    public class DelayJob : IJob
    {
        public static int Executed = 0;

        public void Execute(CancellationToken token)
        {
            Executed++;
        }
    }
}
