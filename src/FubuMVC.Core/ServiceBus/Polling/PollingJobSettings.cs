using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FubuCore.Util;
using FubuMVC.Core.Diagnostics.Packaging;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.ServiceBus.Events;
using FubuMVC.Core.ServiceBus.Monitoring;
using FubuMVC.Core.ServiceBus.Runtime.Delayed;
using FubuMVC.Core.ServiceBus.Subscriptions;

namespace FubuMVC.Core.ServiceBus.Polling
{
    public class PollingJobSettings : IFeatureSettings, IChainSource
    {
        private readonly Cache<Type, PollingJobChain> _jobs =
            new Cache<Type, PollingJobChain>();

        public IEnumerable<PollingJobChain> Jobs
        {
            get { return _jobs.Concat(BuiltInJobs()); }
        }

        public PollingJobChain JobFor<T>() where T : IJob
        {
            return _jobs[typeof (T)];
        }

        public static IEnumerable<PollingJobChain> BuiltInJobs()
        {
            yield return
                PollingJobChain.For<DelayedEnvelopeProcessor, TransportSettings>(x => x.DelayMessagePolling);

            yield return
                PollingJobChain.For<ExpiringListenerCleanup, TransportSettings>(x => x.ListenerCleanupPolling);

            yield return
                PollingJobChain.For<HealthMonitorPollingJob, HealthMonitoringSettings>(x => x.Interval);

            yield return
                PollingJobChain.For<SubscriptionRefreshJob, TransportSettings>(x => x.SubscriptionRefreshPolling);
        }

        void IFeatureSettings.Apply(FubuRegistry registry)
        {
            Jobs.Select(x => x.ToInstance())
                .Each(x => registry.Services.AddService(typeof (IPollingJob), x));

            registry.Policies.ChainSource(this);
        }

        public PollingJobChain AddJob<TJob, TSettings>(Expression<Func<TSettings, double>> intervalSource) where TJob : IJob
        {
            var definition = PollingJobChain.For<TJob, TSettings>(intervalSource);
            _jobs[typeof (TJob)] = definition;

            return definition;
        }

        public void AddJob(PollingJobChain jobChain)
        {
            _jobs[jobChain.JobType] = jobChain;
        }

        public Task<BehaviorChain[]> BuildChains(BehaviorGraph graph, IPerfTimer timer)
        {
            var chains = Jobs.OfType<BehaviorChain>().ToArray();
            return Task.FromResult(chains);
        }
    }
}