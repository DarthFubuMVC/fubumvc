using System;
using System.Collections.Generic;
using FubuTransportation.Configuration;

namespace FubuTransportation.Subscriptions
{
    public interface ISubscriptionRequirement
    {
        IEnumerable<Subscription> DetermineRequirements();
    }

    public interface ISubscriptionRequirement<T>
    {
        IEnumerable<Subscription> Determine(T settings, ChannelGraph graph);
        void AddType(Type type);
    }
}