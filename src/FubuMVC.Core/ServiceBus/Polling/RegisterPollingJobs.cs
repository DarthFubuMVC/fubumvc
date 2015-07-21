using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using FubuMVC.Core.Registration;
using FubuMVC.Core.ServiceBus.Events;
using FubuMVC.Core.ServiceBus.Monitoring;
using FubuMVC.Core.ServiceBus.Runtime.Delayed;
using FubuMVC.Core.ServiceBus.Subscriptions;

namespace FubuMVC.Core.ServiceBus.Polling
{
    [Description("Adds the configured polling jobs to the application services")]
    public class RegisterPollingJobs : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            var jobs = graph.Settings.Get<PollingJobSettings>().Jobs;

            jobs.Select(x => x.ToObjectDef())
                .Each(x => graph.Services.AddService(typeof(IPollingJob), x));

            builtInJobs().Select(x => x.ToObjectDef())
                .Each(x => graph.Services.AddService(typeof (IPollingJob), x));
        }

        private IEnumerable<PollingJobDefinition> builtInJobs()
        {
            yield return
                PollingJobDefinition.For<DelayedEnvelopeProcessor, TransportSettings>(x => x.DelayMessagePolling);

            yield return
                PollingJobDefinition.For<ExpiringListenerCleanup, TransportSettings>(x => x.ListenerCleanupPolling);

            yield return
                PollingJobDefinition.For<HealthMonitorPollingJob, HealthMonitoringSettings>(x => x.Interval);

            yield return
                PollingJobDefinition.For<SubscriptionRefreshJob, TransportSettings>(x => x.SubscriptionRefreshPolling);
        }
    }
}