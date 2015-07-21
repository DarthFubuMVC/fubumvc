using System;
using System.Collections.Generic;

namespace FubuMVC.Core.ServiceBus.Subscriptions
{
    // TODO -- rename to ITransportNodeRepository
    public interface ISubscriptionRepository
    {
        void PersistSubscriptions(params Subscription[] requirements);
        IEnumerable<Subscription> LoadSubscriptions(SubscriptionRole role);
        IEnumerable<TransportNode> FindPeers();
        void PersistPublishing(params Subscription[] subscriptions);

        void Persist(params TransportNode[] nodes);

        TransportNode FindLocal();
        TransportNode FindPeer(string nodeId);

        void AddOwnershipToThisNode(Uri subject);
        void AddOwnershipToThisNode(IEnumerable<Uri> subjects);
        void RemoveOwnershipFromThisNode(Uri subject);
        void RemoveOwnershipFromNode(string nodeId, Uri subject);
        void RemoveOwnershipFromNode(string nodeId, IEnumerable<Uri> subjects);
        void RemoveOwnershipFromThisNode(IEnumerable<Uri> subjects);
        IEnumerable<Subscription> RemoveLocalSubscriptions();
        void RemoveSubscriptionsForReceiver(Uri receiver);
    }
}