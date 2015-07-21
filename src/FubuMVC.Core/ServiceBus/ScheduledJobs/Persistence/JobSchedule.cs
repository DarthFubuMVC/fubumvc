using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FubuCore.Util;

namespace FubuMVC.Core.ServiceBus.ScheduledJobs.Persistence
{
    public class JobSchedule : IEnumerable<JobStatus>
    {
        private readonly Cache<Type, JobStatus> _status =
            new Cache<Type, JobStatus>(x => new JobStatus(x));


        public JobSchedule()
        {
        }

        public JobSchedule(IEnumerable<JobStatus> all)
        {
            all.Each(x => _status[x.JobType] = x);
        }

        public JobStatus Find(Type jobType)
        {
            return _status[jobType];
        }

        public JobStatus Schedule(Type jobType, DateTimeOffset nextTime)
        {
            var status = _status[jobType];
            status.NextTime = nextTime;

            return status;
        }

        public void RemoveObsoleteJobs(IEnumerable<Type> jobTypes)
        {
            _status.Where(x => !jobTypes.Contains(x.JobType)).Each(x => {
                x.Status = JobExecutionStatus.Inactive;
                x.NextTime = null;
            });
        }

        public IEnumerator<JobStatus> GetEnumerator()
        {
            return _status.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerable<JobStatus> Active()
        {
            return _status.Where(x => x.Status != JobExecutionStatus.Inactive);
        }
    }
}