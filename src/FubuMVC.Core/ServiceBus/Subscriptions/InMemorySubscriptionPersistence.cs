using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using FubuCore;
using FubuCore.Util;

namespace FubuMVC.Core.ServiceBus.Subscriptions
{
    public class InMemorySubscriptionPersistence : ISubscriptionPersistence
    {
        private readonly Cache<Guid, Subscription> _subscriptions = new Cache<Guid, Subscription>();
        private readonly IList<TransportNode> _nodes = new List<TransportNode>();
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();


        public IEnumerable<Subscription> LoadSubscriptions(string name, SubscriptionRole role)
        {
            return _lock.Read(() => {
                return _subscriptions.Where(x => x.NodeName.EqualsIgnoreCase(name) && x.Role == role).ToArray();
            });
        }

        public void Persist(IEnumerable<Subscription> subscriptions)
        {
            _lock.Write(() => subscriptions.Each(persist));
        }

        public void Persist(Subscription subscription)
        {
            _lock.Write(() => persist(subscription));
        }

        private void persist(Subscription subscription)
        {
            if (subscription.Id == Guid.Empty)
            {
                subscription.Id = Guid.NewGuid();
            }

            _subscriptions[subscription.Id] = subscription;
        }

        public IEnumerable<TransportNode> NodesForGroup(string name)
        {
            return _lock.Read(() => {
                return _nodes.Where(x => x.NodeName.EqualsIgnoreCase(name));
            });
        }

        public void Persist(params TransportNode[] nodes)
        {
            _lock.Write(() => {
                nodes.Each(node =>
                {
                    if (node.Id.IsEmpty())
                    {
                        throw new ArgumentException("An Id string is required", "node");
                    }

                    _nodes.Fill(node);
                });
            });
        }

        public IEnumerable<TransportNode> AllNodes()
        {
            return _lock.Read(() => _nodes.ToArray());
        }

        public IEnumerable<Subscription> AllSubscriptions()
        {
            return _lock.Read(() => {
                return _subscriptions.ToArray();
            });
        }

        public TransportNode LoadNode(string nodeId)
        {
            return _lock.Read(() => {
                return _nodes.FirstOrDefault(x => x.Id == nodeId);
            });
        }

        public void Alter(string id, Action<TransportNode> alteration)
        {
            _lock.Write(() => {
                var node = _nodes.FirstOrDefault(x => x.Id == id);
                alteration(node);
            });
        }

        public void DeleteSubscriptions(IEnumerable<Subscription> subscriptions)
        {
            _lock.Write(() => subscriptions.Each(x => _subscriptions.Remove(x.Id)));
        }
    }
}