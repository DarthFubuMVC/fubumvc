using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore.Reflection;
using FubuMVC.Core.Registration;
using FubuTransportation.Polling;
using FubuTransportation.ScheduledJobs.Execution;
using FubuTransportation.ScheduledJobs.Persistence;

namespace FubuTransportation.ScheduledJobs
{
    [ApplicationLevel]
    public class ScheduledJobGraph
    {
        public ScheduledJobGraph()
        {
            MaxJobExecutionRecordsToKeepInHistory = 50;
        }


        public readonly IList<IScheduledJob> Jobs = new List<IScheduledJob>();
        public Accessor DefaultChannel { get; set; }

        public int MaxJobExecutionRecordsToKeepInHistory { get; set; }

        public void DetermineSchedule(DateTimeOffset now, IJobExecutor executor, JobSchedule schedule)
        {
            // Make sure that all existing jobs are schedules
            Jobs.Each(x => x.Initialize(now, executor, schedule));

            var types = Jobs.Select(x => x.JobType).ToArray();
            schedule.RemoveObsoleteJobs(types);
        }

        public IScheduledJob FindJob(Type jobType)
        {
            return Jobs.FirstOrDefault(x => x.JobType == jobType);
        }

        public IScheduledJob<T> FindJob<T>() where T : IJob
        {
            return Jobs.OfType<IScheduledJob<T>>().FirstOrDefault();
        } 
    }
}