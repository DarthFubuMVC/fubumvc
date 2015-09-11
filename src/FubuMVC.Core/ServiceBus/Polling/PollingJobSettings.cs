using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FubuCore.Util;
using FubuMVC.Core.Registration;
using FubuMVC.Core.ServiceBus.Events;
using FubuMVC.Core.ServiceBus.Monitoring;
using FubuMVC.Core.ServiceBus.Runtime.Delayed;
using FubuMVC.Core.ServiceBus.Subscriptions;

namespace FubuMVC.Core.ServiceBus.Polling
{
    public class PollingJobSettings : IFeatureSettings
    {
        private readonly Cache<Type, PollingJobDefinition> _jobs =
            new Cache<Type, PollingJobDefinition>();

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
            Jobs.Select(x => x.ToInstance())
                .Each(x => registry.Services.AddService(typeof (IPollingJob), x));
        }

        public PollingJobDefinition AddJob<TJob, TSettings>(Expression<Func<TSettings, double>> intervalSource) where TJob : IJob
        {
            var definition = PollingJobDefinition.For<TJob, TSettings>(intervalSource);
            _jobs[typeof (TJob)] = definition;

            return definition;
        }

        public void AddJob(PollingJobDefinition jobDefinition)
        {
            _jobs[jobDefinition.JobType] = jobDefinition;
        }
    }
}