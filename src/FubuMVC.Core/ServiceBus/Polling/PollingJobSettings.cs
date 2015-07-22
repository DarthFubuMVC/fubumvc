using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore.Util;
using FubuMVC.Core.Registration;
using FubuMVC.Core.ServiceBus.Events;
using FubuMVC.Core.ServiceBus.Monitoring;
using FubuMVC.Core.ServiceBus.Runtime.Delayed;
using FubuMVC.Core.ServiceBus.Subscriptions;

namespace FubuMVC.Core.ServiceBus.Polling
{
    [ApplicationLevel]
    public class PollingJobSettings : IFeatureSettings
    {
        private readonly Cache<Type, PollingJobDefinition> _jobs =
            new Cache<Type, PollingJobDefinition>(type => new PollingJobDefinition {JobType = type});

        public IEnumerable<PollingJobDefinition> Jobs
        {
            get { return _jobs.Concat(BuiltInJobs()); }
        }

        public PollingJobDefinition JobFor<T>() where T : IJob
        {
            return _jobs[typeof (T)];
        }

        public static IEnumerable<PollingJobDefinition> BuiltInJobs()
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

        void IFeatureSettings.Apply(FubuRegistry registry)
        {
            registry.Services(_ => Jobs.Select(x => x.ToObjectDef())
                .Each(x => _.AddService(typeof (IPollingJob), x)));
        }
    }
}