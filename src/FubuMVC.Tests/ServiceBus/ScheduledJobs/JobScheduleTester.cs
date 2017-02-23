using System;
using FubuMVC.Core.ServiceBus.ScheduledJobs.Persistence;
using Xunit;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus.ScheduledJobs
{
    
    public class JobScheduleTester
    {
        [Fact]
        public void remove_obsolete_jobs()
        {
            var schedule = new JobSchedule(new[]
            {
                new JobStatus(typeof (AJob), DateTime.Today),
                new JobStatus(typeof (BJob), DateTime.Today),
            });

            schedule.RemoveObsoleteJobs(new Type[]{typeof(AJob), typeof(CJob)});
            schedule.Find(typeof (BJob)).Status.ShouldBe(JobExecutionStatus.Inactive);
        }


    }
}