using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Diagnostics.Packaging;
using FubuMVC.Core.ServiceBus.Configuration;

namespace FubuMVC.Core.ServiceBus.Polling
{
    public class PollingJobActivator : IActivator
    {
        private readonly IPollingJobs _jobs;
        private readonly ChannelGraph _channels;

        public PollingJobActivator(IPollingJobs jobs, ChannelGraph channels)
        {
            _jobs = jobs;
            _channels = channels;
        }

        public void Activate(IActivationLog log, IPerfTimer timer)
        {
            _jobs.Where(x => x.ScheduledExecution != ScheduledExecution.Disabled).Each(x => {
                try
                {
                    log.Trace($"Starting {x.JobType.GetFullName()} on node {_channels.NodeId}");
                    Debug.WriteLine($"Starting {x.JobType.GetFullName()} on node {_channels.NodeId}");

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