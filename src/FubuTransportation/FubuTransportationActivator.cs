using System.Collections.Generic;
using FubuMVC.Core;
using FubuMVC.Core.Diagnostics.Packaging;
using FubuTransportation.Polling;
using FubuTransportation.Runtime;
using FubuTransportation.ScheduledJobs.Execution;
using FubuTransportation.Subscriptions;

namespace FubuTransportation
{
    public class FubuTransportationActivator : IActivator, IDeactivator
    {
        private readonly TransportActivator _transports;
        private readonly SubscriptionActivator _subscriptions;
        private readonly IScheduledJobController _scheduledJobs;
        private readonly PollingJobActivator _pollingJobs;
        private readonly TransportSettings _settings;

        public FubuTransportationActivator(TransportActivator transports, SubscriptionActivator subscriptions,
            IScheduledJobController scheduledJobs, PollingJobActivator pollingJobs, TransportSettings settings)
        {
            _transports = transports;
            _subscriptions = subscriptions;
            _scheduledJobs = scheduledJobs;
            _pollingJobs = pollingJobs;
            _settings = settings;
        }

        public void Activate(IPackageLog log)
        {
            if (_settings.Disabled)
            {
                log.Trace("Skipping activation because FubuTranportation is disabled.");
                return;
            }

            _transports.Activate(log);
            _subscriptions.Activate(log);
            _pollingJobs.Activate(log);

            /* TODO -- add timings back here!
            PackageRegistry.Timer.Record("Activating Transports and Starting Listening",
                () => _transports.Activate(packages, log));

            PackageRegistry.Timer.Record("Activating Subscriptions", () => _subscriptions.Activate(packages, log));

            PackageRegistry.Timer.Record("Activating Polling Jobs", () => _pollingJobs.Activate(packages, log));
             * */
        }

        public void Deactivate(IPackageLog log)
        {
            if(_settings.Disabled) return;

            log.Trace("Shutting down the scheduled jobs");
            _scheduledJobs.Deactivate();
        }
    }
}
