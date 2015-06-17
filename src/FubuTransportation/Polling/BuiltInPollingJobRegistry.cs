using FubuTransportation.Configuration;
using FubuTransportation.Events;
using FubuTransportation.Monitoring;
using FubuTransportation.Runtime.Delayed;
using FubuTransportation.Subscriptions;

namespace FubuTransportation.Polling
{
    public class BuiltInPollingJobRegistry : FubuTransportRegistry
    {
        public BuiltInPollingJobRegistry()
        {
            Handlers.DisableDefaultHandlerSource();
            Polling.RunJob<DelayedEnvelopeProcessor>()
                .ScheduledAtInterval<TransportSettings>(x => x.DelayMessagePolling);

            Polling.RunJob<ExpiringListenerCleanup>()
                .ScheduledAtInterval<TransportSettings>(x => x.ListenerCleanupPolling);

            Polling.RunJob<HealthMonitorPollingJob>()
                .ScheduledAtInterval<HealthMonitoringSettings>(x => x.Interval);

            Polling.RunJob<SubscriptionRefreshJob>()
                .ScheduledAtInterval<TransportSettings>(x => x.SubscriptionRefreshPolling);
        }
    }
}