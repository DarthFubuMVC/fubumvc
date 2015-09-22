using System;
using System.Threading;
using FubuMVC.Core.ServiceBus.Polling;
using FubuMVC.Core.ServiceBus.ScheduledJobs;
using FubuMVC.Core.ServiceBus.ScheduledJobs.Execution;
using FubuMVC.Core.ServiceBus.ScheduledJobs.Persistence;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus.ScheduledJobs
{

    [TestFixture]
    public class when_scheduling_jobs
    {
        private JobSchedule theSchedule;
        private ScheduledJobGraph theGraph;
        private StubJobExecutor theExecutor;
        private readonly DateTimeOffset now = DateTime.Today.AddHours(-1);

        [SetUp]
        public void SetUp()
        {
            theExecutor = new StubJobExecutor();

            theSchedule = new JobSchedule(new[]
            {
                JobStatus.For<AJob>(DateTime.Today),
                JobStatus.For<BJob>(DateTime.Today.AddHours(1)),
                JobStatus.For<CJob>(DateTime.Today.AddHours(2)),
            });

            theGraph = new ScheduledJobGraph();
            theGraph.Jobs.Add(new ScheduledJob<BJob>(new DummyScheduleRule(DateTime.Today.AddHours(1))));
            theGraph.Jobs.Add(new ScheduledJob<CJob>(new DummyScheduleRule(DateTime.Today.AddHours(3))));
            theGraph.Jobs.Add(new ScheduledJob<DJob>(new DummyScheduleRule(DateTime.Today.AddHours(4))));
            theGraph.Jobs.Add(new ScheduledJob<EJob>(new DummyScheduleRule(DateTime.Today.AddHours(5))));

            // not that worried about pushing the time around
            theGraph.DetermineSchedule(now, theExecutor, theSchedule);
        }

        [Test]
        public void changes_the_jobs_that_are_already_scheduled_correcting_where_necessary()
        {
            theSchedule.Find(typeof (CJob)).NextTime.ShouldBe((DateTimeOffset) DateTime.Today.AddHours(3));
            theSchedule.Find(typeof (BJob)).NextTime.ShouldBe((DateTimeOffset) DateTime.Today.AddHours(1));
        }

        [Test]
        public void schedules_new_jobs()
        {
            theSchedule.Find(typeof (DJob)).NextTime.ShouldBe((DateTimeOffset) DateTime.Today.AddHours(4));
            theSchedule.Find(typeof (EJob)).NextTime.ShouldBe((DateTimeOffset) DateTime.Today.AddHours(5));
        }

        [Test]
        public void removes_obsolete_jobs()
        {
            theSchedule.Find(typeof (AJob)).Status.ShouldBe(JobExecutionStatus.Inactive);
        }
    }

    public class DummyScheduleRule : IScheduleRule
    {
        private readonly DateTimeOffset _nextTime;

        public DummyScheduleRule()
        {
            _nextTime = DateTimeOffset.UtcNow.AddDays(1);
        }

        public DummyScheduleRule(DateTimeOffset nextTime)
        {
            _nextTime = nextTime;
        }

        public DateTimeOffset ScheduleNextTime(DateTimeOffset currentTime, JobExecutionRecord lastExecution)
        {
            return _nextTime;
        }
    }

    public class DJob : IJob
    {
        public void Execute(CancellationToken cancellation)
        {
            throw new System.NotImplementedException();
        }
    }

    public class EJob : IJob
    {
        public void Execute(CancellationToken cancellation)
        {
            throw new System.NotImplementedException();
        }
    }
}