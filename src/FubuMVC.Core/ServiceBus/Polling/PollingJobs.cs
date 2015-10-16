using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FubuCore;
using StructureMap;


namespace FubuMVC.Core.ServiceBus.Polling
{
    [Singleton]
    public class PollingJobs : IPollingJobs
    {
        private readonly IEnumerable<IPollingJob> _jobs;

        public PollingJobs(IEnumerable<IPollingJob> jobs)
        {
            _jobs = jobs;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<IPollingJob> GetEnumerator()
        {
            return _jobs.GetEnumerator();
        }

        public bool IsActive<T>() where T : IJob
        {
            return IsActive(typeof (T));
        }

        public IPollingJob For(Type jobType)
        {
            return _jobs.FirstOrDefault(x => x.JobType == jobType);
        }

        public bool IsActive(Type jobType)
        {
            var job = For(jobType);
            return job == null ? false : job.IsRunning();
        }

        public void Activate<T>() where T : IJob
        {
            Activate(typeof(T));
        }

        public void Activate(Type type)
        {
            var job = For(type);
            if (job != null) job.Start();
        }

        public Task WaitForJobToExecute<T>() where T : IJob
        {
            var job = For(typeof(T));
            if (job == null) throw new ArgumentOutOfRangeException("T", "Unknown job type " + typeof(T).GetFullName());

            return job.WaitForJobToExecute();
        }

        public Task ExecuteJob<T>() where T : IJob
        {
            var job = For(typeof(T));
            if (job == null) throw new ArgumentOutOfRangeException("T", "Unknown job type " + typeof(T).GetFullName());

            return Task.Factory.StartNew(() => {
                job.RunNow();
            });
        }
    }
}