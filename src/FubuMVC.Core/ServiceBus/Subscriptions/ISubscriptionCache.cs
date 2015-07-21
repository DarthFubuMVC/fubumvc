using System;
using System.Collections.Generic;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.Runtime;

namespace FubuMVC.Core.ServiceBus.Subscriptions
{
    public interface ISubscriptionCache
    {
        void ClearAll();

        IEnumerable<ChannelNode> FindDestinationChannels(Envelope envelope);

        Uri ReplyUriFor(ChannelNode destination);

        /// <summary>
        /// Called internally
        /// </summary>
        /// <param name="subscriptions"></param>
        void LoadSubscriptions(IEnumerable<Subscription> subscriptions);

        IEnumerable<Subscription> ActiveSubscriptions { get; }

        string NodeName { get; }
    }
}