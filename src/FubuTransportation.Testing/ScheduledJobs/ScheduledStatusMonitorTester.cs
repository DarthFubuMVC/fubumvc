using System;
using System.Linq;
using System.Threading;
using FubuCore.Dates;
using FubuCore.Logging;
using FubuTestingSupport;
using FubuTransportation.Configuration;
using FubuTransportation.Polling;
using FubuTransportation.ScheduledJobs;
using FubuTransportation.ScheduledJobs.Persistence;
using NUnit.Framework;

namespace FubuTransportation.Testing.ScheduledJobs
{
    [TestFixture]
    public class ScheduledStatusMonitorTester
    {
        private JobStatusDTO foo1;
        private JobStatusDTO foo2;
        private JobStatusDTO foo3;
        private JobStatusDTO bar1;
        private JobStatusDTO bar2;
        private InMemorySchedulePersistence thePersistence;
        private RecordingLogger theLogger;
        private ScheduleStatusMonitor theStatusMonitor;
        private ChannelGraph theChannelGraph = new ChannelGraph {Name = "foo"};

        [SetUp]
        public void SetUp()
        {
            foo1 = new JobStatusDTO { JobKey = "1", NodeName = "foo" };
            foo2 = new JobStatusDTO { JobKey = "2", NodeName = "foo" };
            foo3 = new JobStatusDTO { JobKey = "3", NodeName = "foo" };
            bar1 = new JobStatusDTO { JobKey = "1", NodeName = "bar" };
            bar2 = new JobStatusDTO { JobKey = "2", NodeName = "bar" };

            thePersistence = new InMemorySchedulePersistence();
            thePersistence.Persist(new[] { foo1, foo2, foo3, bar1, bar2 });

            theLogger = new RecordingLogger();

            theStatusMonitor = new ScheduleStatusMonitor(theChannelGraph, new ScheduledJobGraph(), thePersistence, theLogger, SystemTime.Default());

        }

        [Test]
        public void mark_scheduled_persistence()
        {
            var next = (DateTimeOffset)DateTime.Today;
            theStatusMonitor.MarkScheduled<FooJob1>(next);

            foo1.Status.ShouldEqual(JobExecutionStatus.Scheduled);
            foo1.NextTime.ShouldEqual(next);
        }

        [Test]
        public void mark_executing_persistence()
        {
            foo1.Executor = null;

            theStatusMonitor.MarkExecuting<FooJob1>();

            foo1.Status.ShouldEqual(JobExecutionStatus.Executing);
            foo1.Executor.ShouldEqual(theChannelGraph.NodeId);

        }

        [Test]
        public void mark_completion_persistence()
        {
            var record = new JobExecutionRecord{Success = true};
            theStatusMonitor.MarkCompletion<FooJob1>(record);

            foo1.Status.ShouldEqual(JobExecutionStatus.Completed);
            foo1.LastExecution.ShouldBeTheSameAs(record);
            foo1.Executor.ShouldBeNull();

            record.Executor.ShouldEqual(theChannelGraph.NodeId);

            thePersistence.FindHistory(foo1.NodeName, foo1.JobKey)
                .ShouldHaveTheSameElementsAs(record);
        }

        [Test]
        public void track_and_succeed()
        {
            var tracker = theStatusMonitor.TrackJob(3, new FooJob1());
            var next = (DateTimeOffset)DateTime.Today.AddHours(3);
            
            tracker.Success(next);

            foo1.Status.ShouldEqual(JobExecutionStatus.Completed);
            foo1.LastExecution.Executor.ShouldEqual(theChannelGraph.NodeId);
            foo1.LastExecution.Success.ShouldBeTrue();
            foo1.LastExecution.Attempts.ShouldEqual(3);
            foo1.NextTime.ShouldEqual(next);
            
            foo1.Executor.ShouldBeNull();

            foo1.LastExecution.Executor.ShouldEqual(theChannelGraph.NodeId);
        }

        [Test]
        public void track_and_fail()
        {
            var tracker = theStatusMonitor.TrackJob(3, new FooJob1());

            var ex = new NotImplementedException();
            tracker.Failure(ex);

            foo1.Status.ShouldEqual(JobExecutionStatus.Failed);
            foo1.LastExecution.Executor.ShouldEqual(theChannelGraph.NodeId);
            foo1.LastExecution.Success.ShouldBeFalse();
            foo1.LastExecution.Attempts.ShouldEqual(3);
            foo1.LastExecution.ExceptionText.ShouldEqual(ex.ToString());

            foo1.Executor.ShouldBeNull();

            thePersistence.FindHistory(foo1.NodeName, foo1.JobKey)
                .Single().ShouldBeTheSameAs(foo1.LastExecution);

            foo1.LastExecution.Executor.ShouldEqual(theChannelGraph.NodeId);
        }

        [Test]
        public void mark_completion_with_failure_persistence()
        {
            var record = new JobExecutionRecord
            {
                Success = false
            };
            theStatusMonitor.MarkCompletion<FooJob1>(record);

            foo1.Status.ShouldEqual(JobExecutionStatus.Failed);
            foo1.LastExecution.ShouldBeTheSameAs(record);
            foo1.Executor.ShouldBeNull();

            record.Executor.ShouldEqual(theChannelGraph.NodeId);
        }




        [JobKey("1")]
        public class FooJob1 : IJob
        {
            public void Execute(CancellationToken cancellation)
            {
                throw new System.NotImplementedException();
            }
        }
    }
}