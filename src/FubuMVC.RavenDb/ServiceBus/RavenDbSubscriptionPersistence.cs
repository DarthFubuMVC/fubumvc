using System;
using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.ServiceBus.Subscriptions;
using Raven.Client;

namespace FubuMVC.RavenDb.ServiceBus
{
    public class RavenDbSubscriptionPersistence : ISubscriptionPersistence
    {
        private readonly ITransaction _transaction;
        private readonly IDocumentStore _store;

        public RavenDbSubscriptionPersistence(ITransaction transaction, IDocumentStore store)
        {
            _transaction = transaction;
            _store = store;
        }

        public IEnumerable<Subscription> LoadSubscriptions(string name, SubscriptionRole role)
        {
            using (var session = _store.OpenSession())
            {
                return session.Query<Subscription>()
                    .Customize(x => x.WaitForNonStaleResultsAsOfLastWrite())
                    .Where(x => x.NodeName == name && x.Role == role)
                    .ToList();
            }
        }

        public void Persist(IEnumerable<Subscription> subscriptions)
        {
            _transaction.Execute<IDocumentSession>(session => subscriptions.Each(s => session.Store(s)));
        }

        public void Persist(Subscription subscription)
        {
            _transaction.Execute<IDocumentSession>(x => x.Store(subscription));
        }

        public IEnumerable<TransportNode> NodesForGroup(string name)
        {
            using (var session = _store.OpenSession())
            {
                return session
                    .Query<TransportNode>()
                    .Customize(x => x.WaitForNonStaleResultsAsOfLastWrite())
                    .Where(x => x.NodeName == name)
                    .ToList();
            }
        }

        public void Persist(params TransportNode[] nodes)
        {
            _transaction.Execute<IDocumentSession>(x => {
                nodes.Each(node => x.Store(node));
            });
        }

        public IEnumerable<TransportNode> AllNodes()
        {
            using (var session = _store.OpenSession())
            {

                return session.Query<TransportNode>()
                    .Customize(x => x.WaitForNonStaleResultsAsOfLastWrite())
                    .ToArray();
            }
        }

        public IEnumerable<Subscription> AllSubscriptions()
        {
            using (var session = _store.OpenSession())
            {
                return session
                    .Query<Subscription>()
                    .Customize(x => x.WaitForNonStaleResultsAsOfLastWrite()).ToArray();
            }
        }

        public TransportNode LoadNode(string nodeId)
        {
            using (var session = _store.OpenSession())
            {
                return session.Load<TransportNode>(nodeId);
            }
        }

        public void Alter(string id, Action<TransportNode> alteration)
        {
            _transaction.Execute<IDocumentSession>(session => {
                var node = session.Load<TransportNode>(id);
                if (node != null)
                {
                    alteration(node);
                    session.Store(node);
                }
            });
        }

        public void DeleteSubscriptions(IEnumerable<Subscription> subscriptions)
        {
            _transaction.Execute<IDocumentSession>(session => {
                var docs = session.Load<Subscription>(subscriptions.Select(x => x.Id).Cast<ValueType>());
                docs.Each(x => session.Delete(x));
            });
        }
    }
}