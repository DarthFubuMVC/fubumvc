using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FubuCore.Descriptions;
using FubuCore.Util;
using FubuMVC.Core.Diagnostics.Packaging;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.ServiceBus.Polling
{
    [Description("See the polling jobs diagnostic screen")]
    public class PollingJobSettings : IFeatureSettings, IChainSource
    {
        private readonly Cache<Type, PollingJobChain> _jobs =
            new Cache<Type, PollingJobChain>();

        public IEnumerable<PollingJobChain> Jobs => _jobs;

        public Task<BehaviorChain[]> BuildChains(BehaviorGraph graph, IPerfTimer timer)
        {
            var chains = Jobs.OfType<BehaviorChain>().ToArray();
            return Task.FromResult(chains);
        }

        void IFeatureSettings.Apply(FubuRegistry registry)
        {
            Jobs.Select(x => x.ToInstance())
                .Each(x => registry.Services.AddService(typeof(IPollingJob), x));

            registry.Policies.ChainSource(this);
        }

        public PollingJobChain JobFor<T>() where T : IJob
        {
            return _jobs[typeof(T)];
        }

        public bool HasJob<T>()
        {
            return _jobs.Has(typeof(T));
        }

        public PollingJobChain AddJob<TJob, TSettings>(Expression<Func<TSettings, double>> intervalSource)
            where TJob : IJob
        {
            var definition = PollingJobChain.For<TJob, TSettings>(intervalSource);
            _jobs[typeof(TJob)] = definition;

            return definition;
        }

        public void AddJob(PollingJobChain jobChain)
        {
            _jobs[jobChain.JobType] = jobChain;
        }
    }
}