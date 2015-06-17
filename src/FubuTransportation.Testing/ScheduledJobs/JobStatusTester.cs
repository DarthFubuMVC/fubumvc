using System;
using System.Threading;
using FubuCore;
using FubuCore.Dates;
using FubuTestingSupport;
using FubuTransportation.Polling;
using FubuTransportation.ScheduledJobs;
using FubuTransportation.ScheduledJobs.Persistence;
using NUnit.Framework;

namespace FubuTransportation.Testing.ScheduledJobs
{
    [TestFixture]
    public class JobStatusTester
    {
        [Test]
        public void to_status_without_any_attribute()
        {
            var status = new JobStatus(typeof (AJob), DateTime.Today)
            {
                NextTime = DateTime.Today.AddHours(1),
                Status = JobExecutionStatus.Scheduled,
                LastExecution = new JobExecutionRecord
                {
                    Duration = 123,
                    ExceptionText = null,
                    Finished = DateTime.Today.AddHours(-1),
                    Success = true,
                    
                }
            };

            var dto = status.ToDTO("foo");
            dto.NodeName.ShouldEqual("foo");
            dto.JobKey.ShouldEqual("AJob");
            dto.LastExecution.ShouldEqual(status.LastExecution);
            dto.NextTime.ShouldEqual(status.NextTime);
            dto.Status.ShouldEqual(JobExecutionStatus.Scheduled);
        }

        [Test]
        public void to_status_with_attribute_on_job_type()
        {
            var status = new JobStatus(typeof(DecoratedJob), DateTime.Today)
            {
                Status = JobExecutionStatus.Executing,
                NextTime = DateTime.Today.AddHours(1),
                LastExecution = new JobExecutionRecord
                {
                    Duration = 123,
                    ExceptionText = null,
                    Finished = DateTime.Today.AddHours(-1),
                    Success = true
                }
            };

            var dto = status.ToDTO("foo");
            dto.NodeName.ShouldEqual("foo");
            dto.JobKey.ShouldEqual("Special");
            dto.LastExecution.ShouldEqual(status.LastExecution);
            dto.NextTime.ShouldEqual(status.NextTime);
            dto.Status.ShouldEqual(JobExecutionStatus.Executing);
        }
    }

    [JobKey("Special")]
    public class DecoratedJob : IJob
    {
        public void Execute(CancellationToken cancellation)
        {
            throw new NotImplementedException();
        }
    }
}