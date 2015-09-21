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
            get { return _jobs; }
        }

        public PollingJobChain JobFor<T>() where T : IJob
        {
            return _jobs[typeof (T)];
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