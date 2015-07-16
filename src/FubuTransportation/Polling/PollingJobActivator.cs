using System;
using System.Collections.Generic;
using System.Linq;
using Bottles;
using Bottles.Diagnostics;
using FubuCore;
using FubuCore.Descriptions;

namespace FubuTransportation.Polling
{
    public class PollingJobActivator : IActivator
    {
        private readonly IPollingJobs _jobs;

        public PollingJobActivator(IPollingJobs jobs)
        {
            _jobs = jobs;
        }

        public void Activate(IPackageLog log)
        {
            _jobs.Where(x => x.ScheduledExecution != ScheduledExecution.Disabled).Each(x => {
                try
                {
                    log.Trace("Starting " + x.JobType.GetFullName());
                    x.Start();
                }
                catch (Exception ex)
                {
                    log.MarkFailure(ex);
                }
            });
        }
    }
}