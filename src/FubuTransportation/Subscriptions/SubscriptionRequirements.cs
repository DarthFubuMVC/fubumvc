using System.Collections.Generic;
using System.Linq;
using FubuTransportation.Configuration;

namespace FubuTransportation.Subscriptions
{
    public class SubscriptionRequirements<T> : ISubscriptionRequirement
    {
        private readonly T _settings;
        private readonly ChannelGraph _graph;
        private readonly IList<ISubscriptionRequirement<T>> _requirements;

        public SubscriptionRequirements(T settings, ChannelGraph graph, IList<ISubscriptionRequirement<T>> requirements)
        {
            _settings = settings;
            _graph = graph;
            _requirements = requirements;
        }

        public IEnumerable<Subscription> DetermineRequirements()
        {
            return _requirements.SelectMany(x => x.Determine(_settings, _graph));
        }
    }
}