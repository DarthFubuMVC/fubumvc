using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Diagnostics.Packaging;

namespace FubuMVC.Core.ServiceBus.Polling
{
    public class PollingJobActivator : IActivator
    {
        private readonly IPollingJobs _jobs;

        public PollingJobActivator(IPollingJobs jobs)
        {
            _jobs = jobs;
        }

        public void Activate(IActivationLog log, IPerfTimer timer)
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