using System;
using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.ServiceBus.Subscriptions;
using Raven.Abstractions.Indexing;
using Raven.Client;
using Raven.Client.Linq;

namespace FubuMVC.RavenDb.ServiceBus
{
    public class RavenDbSubscriptionPersistence : ISubscriptionPersistence
    {
        private readonly ITransaction _transaction;
        private readonly IDocumentStore _store;
        private const string SubscriptionIndex = "Subscriptions/ByNodeNameAndRole";

        public RavenDbSubscriptionPersistence(ITransaction transaction, IDocumentStore store)
        {
            _transaction = transaction;
            _store = store;
            PushSubscriptionIndex();
        }

        public void PushSubscriptionIndex()
        {
            _store.DatabaseCommands.PutIndex(SubscriptionIndex, new IndexDefinition
            {
                Map = "from doc in docs.Subscriptions select new { NodeName = doc.NodeName, Role = doc.Role }",

            }, true);
        }

        public IEnumerable<Subscription> LoadSubscriptions(string name, SubscriptionRole role)
        {
            using (var session = _store.OpenSession())
            {
                var query = session.Query<Subscription>(SubscriptionIndex)
                        .Where(x => x.NodeName == name && x.Role == role);
                return Stream(session, query).ToList();
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
                return Stream(session, session.Query<Subscription>(SubscriptionIndex)).ToArray();
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

        private IEnumerable<T> Stream<T>(IDocumentSession session, IRavenQueryable<T> query)
        {
            //Stream doesn't support waiting for non-stale results.
            //Stream also throws an error if the stream is empty.
            var result = query.Customize(x => x.WaitForNonStaleResultsAsOfLastWrite()).FirstOrDefault();
            if (result == null)
            {
                yield break;
            }

            var stream = session.Advanced.Stream(query);
            while (stream.MoveNext())
                yield return stream.Current.Document;
        }
    }
}
