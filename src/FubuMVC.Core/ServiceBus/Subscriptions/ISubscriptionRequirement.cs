using System;
using System.Collections.Generic;
using FubuMVC.Core.ServiceBus.Configuration;

namespace FubuMVC.Core.ServiceBus.Subscriptions
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