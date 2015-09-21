using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.ServiceBus.Polling;
using FubuMVC.Tests.TestSupport;
using NUnit.Framework;
using Shouldly;
using StructureMap;

namespace FubuMVC.Tests.ServiceBus.Polling
{
    [TestFixture]
    public class PollingJobIntegrationTester
    {
        private IContainer container;
        private FubuRuntime theRuntime;

        [TestFixtureSetUp]
        public void SetUp()
        {
            OneJob.Executed = TwoJob.Executed = ThreeJob.Executed = 0;

            theRuntime = FubuRuntime.For<PollingRegistry>();
            theRuntime.Behaviors.PollingJobs.Any().ShouldBeTrue();

            container = theRuntime.Get<IContainer>();

            Wait.Until(() => ThreeJob.Executed > 10, timeoutInMilliseconds: 6000);
        }

        [TestFixtureTearDown]
        public void Teardown()
        {
            theRuntime.Dispose();
        }

        [Test]
        public void the_polling_job_chains_are_tagged_for_no_tracing()
        {
            var graph = theRuntime.Get<BehaviorGraph>();
            var chains = graph.PollingJobs;

            chains.Each(x => x.IsTagged(BehaviorChain.NoTracing).ShouldBeTrue());
        }

        [Test]
        public void there_are_polling_jobs_registered()
        {
            // The polling job for delayed messages & one for the expired listeners are registered by default.
            // Plus 1 for health monitoring
            // And another for subscription refresh

            var pollingJobs = container.GetInstance<IPollingJobs>();

            pollingJobs.Count()
                .ShouldBe(8);
        }

        [Test]
        public void should_have_executed_all_the_jobs_several_times()
        {
            OneJob.Executed.ShouldBeGreaterThan(10);
            TwoJob.Executed.ShouldBeGreaterThan(10);
            ThreeJob.Executed.ShouldBeGreaterThan(10);
        }

        [Test]
        public void
            should_have_executed_one_more_than_two_and_two_more_than_three_because_of_the_polling_interval_differences()
        {
            OneJob.Executed.ShouldBeGreaterThan(TwoJob.Executed);
            TwoJob.Executed.ShouldBeGreaterThan(ThreeJob.Executed);
        }

        [Test]
        public void jobs_that_are_not_disabled_should_be_active()
        {
            var pollingJobs = theRuntime.Get<IPollingJobs>();
            pollingJobs.IsActive<TwoJob>().ShouldBeTrue();
            pollingJobs.IsActive<ThreeJob>().ShouldBeTrue();
        }

        [Test]
        public void disabled_job_should_not_be_active()
        {
            var pollingJobs = theRuntime.Get<IPollingJobs>();
            pollingJobs
                .IsActive<DisabledJob>().ShouldBeFalse();
        }

        [Test]
        public void nonexistent_job_is_not_active()
        {
            var pollingJobs = theRuntime.Get<IPollingJobs>();
            pollingJobs
                .IsActive<MissingJob>().ShouldBeFalse();
        }
    }

    public class MissingJob : IJob
    {
        public void Execute(CancellationToken cancellation)
        {
        }
    }

    public class PollingRegistry : FubuRegistry
    {
        public PollingRegistry()
        {
            ServiceBus.Enable(true);

            ServiceBus.EnableInMemoryTransport();

            Polling.RunJob<OneJob>().ScheduledAtInterval<PollingSettings>(x => x.OneInterval);
            Polling.RunJob<TwoJob>().ScheduledAtInterval<PollingSettings>(x => x.TwoInterval);
            Polling.RunJob<ThreeJob>().ScheduledAtInterval<PollingSettings>(x => x.ThreeInterval).RunImmediately();

            Polling.RunJob<DisabledJob>().ScheduledAtInterval<PollingSettings>(x => x.DisabledInterval).Disabled();

            Services.ReplaceService<IPollingJobLogger, RecordingPollingJobLogger>();
        }
    }

    public class PollingSettings
    {
        public PollingSettings()
        {
            OneInterval = 100;
            TwoInterval = 200;
            ThreeInterval = 300;
            DisabledInterval = 400;
        }

        public double OneInterval { get; set; }
        public double TwoInterval { get; set; }
        public double ThreeInterval { get; set; }
        public double DisabledInterval { get; set; }
    }

    public class OneJob : IJob
    {
        public static int Executed = 0;

        public void Execute(CancellationToken cancellation)
        {
            Executed++;
        }
    }

    public class TwoJob : IJob
    {
        public static int Executed = 0;

        public void Execute(CancellationToken cancellation)
        {
            Executed++;
        }
    }

    public class ThreeJob : IJob
    {
        public static int Executed = 0;

        public void Execute(CancellationToken cancellation)
        {
            Executed++;
        }
    }

    public class DisabledJob : IJob
    {
        public static int Executed = 0;

        public void Execute(CancellationToken cancellation)
        {
            Executed++;
        }
    }

    public class RecordingPollingJobLogger : IPollingJobLogger
    {
        public readonly IList<Type> Stopped = new List<Type>();
        public readonly IList<IJob> Started = new List<IJob>();
        public readonly IList<IJob> Succeeded = new List<IJob>();

        public void Stopping(Type jobType)
        {
            Stopped.Add(jobType);
        }

        public void Starting(Guid id, IJob job)
        {
            Started.Add(job);
        }

        public void Successful(Guid id, IJob job)
        {
            Succeeded.Add(job);
        }

        public void Failed(Guid id, IJob job, Exception ex)
        {
            Assert.Fail("Got an exception for {0}\n{1}", job, ex);
        }

        public void FailedToSchedule(Type jobType, Exception exception)
        {
            Assert.Fail("Failed to schedule {0}\n{1}", jobType, exception);
        }
    }
}