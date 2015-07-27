using System;
using System.Threading;
using FubuMVC.Core.ServiceBus.Polling;
using FubuMVC.Core.ServiceBus.ScheduledJobs;
using FubuMVC.Core.ServiceBus.ScheduledJobs.Persistence;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus.ScheduledJobs
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
            dto.NodeName.ShouldBe("foo");
            dto.JobKey.ShouldBe("AJob");
            dto.LastExecution.ShouldBe(status.LastExecution);
            dto.NextTime.ShouldBe(status.NextTime);
            dto.Status.ShouldBe(JobExecutionStatus.Scheduled);
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
            dto.NodeName.ShouldBe("foo");
            dto.JobKey.ShouldBe("Special");
            dto.LastExecution.ShouldBe(status.LastExecution);
            dto.NextTime.ShouldBe(status.NextTime);
            dto.Status.ShouldBe(JobExecutionStatus.Executing);
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