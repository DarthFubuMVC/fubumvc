using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore.Reflection;
using FubuMVC.Core.Registration;
using FubuMVC.Core.ServiceBus.Polling;
using FubuMVC.Core.ServiceBus.ScheduledJobs.Configuration;
using FubuMVC.Core.ServiceBus.ScheduledJobs.Execution;
using FubuMVC.Core.ServiceBus.ScheduledJobs.Persistence;
using StructureMap.Pipeline;

namespace FubuMVC.Core.ServiceBus.ScheduledJobs
{
    public class ScheduledJobGraph : IFeatureSettings
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

        void IFeatureSettings.Apply(FubuRegistry registry)
        {
            registry.Config.Add(new ScheduledJobHandlerSource(Jobs.Select(x => x.JobType).ToArray()));

            Jobs.Each(x =>
            {
                Type jobType = typeof(IScheduledJob<>).MakeGenericType(x.JobType);
                registry.Services.AddService(jobType, new ObjectInstance(x));

            });
        }
    }
}