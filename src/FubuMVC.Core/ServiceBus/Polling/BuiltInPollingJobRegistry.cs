using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.Events;
using FubuMVC.Core.ServiceBus.Monitoring;
using FubuMVC.Core.ServiceBus.Runtime.Delayed;
using FubuMVC.Core.ServiceBus.Subscriptions;

namespace FubuMVC.Core.ServiceBus.Polling
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