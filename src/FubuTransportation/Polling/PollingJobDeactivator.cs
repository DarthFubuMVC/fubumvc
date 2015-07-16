using System;
using System.Collections.Generic;
using FubuMVC.Core;
using FubuMVC.Core.Diagnostics.Packaging;

namespace FubuTransportation.Polling
{
    public class PollingJobLatch
    {
        public bool Latched;
    }

    public class PollingJobDeactivator : IDeactivator
    {
        private readonly IPollingJobs _jobs;
        private readonly PollingJobLatch _latch;

        public PollingJobDeactivator(IPollingJobs jobs, PollingJobLatch latch)
        {
            _jobs = jobs;
            _latch = latch;
        }

        public void Deactivate(IActivationLog log)
        {
            _latch.Latched = true;

            _jobs.Each(x => {
                try
                {
                    x.Dispose();
                }
                catch (Exception ex)
                {
                    log.MarkFailure(ex);
                }
            });
        }
    }
}