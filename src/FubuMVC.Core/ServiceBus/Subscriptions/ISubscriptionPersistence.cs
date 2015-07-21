using System;
using System.Collections.Generic;

namespace FubuMVC.Core.ServiceBus.Subscriptions
{
    public interface ISubscriptionPersistence
    {
        IEnumerable<Subscription> LoadSubscriptions(string name, SubscriptionRole role);
        void Persist(IEnumerable<Subscription> subscriptions);
        void Persist(Subscription subscription);

        IEnumerable<TransportNode> NodesForGroup(string name);
        void Persist(params TransportNode[] nodes);

        IEnumerable<TransportNode> AllNodes();
        IEnumerable<Subscription> AllSubscriptions();
        TransportNode LoadNode(string nodeId);

        void Alter(string id, Action<TransportNode> alteration);
        void DeleteSubscriptions(IEnumerable<Subscription> subscriptions);
    }
}