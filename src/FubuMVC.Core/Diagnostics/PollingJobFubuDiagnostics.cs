using System.Collections.Generic;
using System.Linq;
using FubuCore.Descriptions;
using FubuMVC.Core.ServiceBus.Polling;

namespace FubuMVC.Core.Diagnostics
{
    public class PollingJobFubuDiagnostics
    {
        private readonly IPollingJobs _jobs;

        public PollingJobFubuDiagnostics(IPollingJobs jobs)
        {
            _jobs = jobs;
        }

        public Dictionary<string, object>[] get_pollingjobs()
        {
            return _jobs.Select(toDictionary).ToArray();
        }

        private Dictionary<string, object> toDictionary(IPollingJob job)
        {
            var dict = new Dictionary<string, object>
            {
                {"chain", job.Chain.Key},
                {"title", Description.For(job).Title},
                {"type", job.JobType.FullName},
                {"interval", job.Interval},
                {"performance", job.Chain.Performance.ToDictionary()},
                {"running", job.IsRunning()},
                {"execution", job.ScheduledExecution.ToString()}
            };


            if (job.Chain.Performance.LastExecution != null)
            {
                dict.Add("last", job.Chain.Performance.LastExecution.ToHeaderDictionary());
            }



            return dict;
        } 
    }

}