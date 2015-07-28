using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using FubuCore;
using FubuCore.Util;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.Runtime;

namespace FubuMVC.Core.ServiceBus.Subscriptions
{
    public class SubscriptionCache : ISubscriptionCache, IDisposable
    {
        private readonly ChannelGraph _graph;
        private readonly IEnumerable<ITransport> _transports;
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
        private readonly IDictionary<Type, IList<ChannelNode>> _routes = new Dictionary<Type, IList<ChannelNode>>();
        private readonly Cache<Uri, ChannelNode> _volatileNodes;
        private readonly IList<Subscription> _subscriptions = new List<Subscription>();

        public SubscriptionCache(ChannelGraph graph, IEnumerable<ITransport> transports)
        {
            if (!transports.Any())
            {
                throw new Exception(
                    "No transports are registered.  FubuMVC's ServiceBus cannot function without at least one ITransport");
            }

            _graph = graph;
            _transports = transports;

            _volatileNodes = new Cache<Uri, ChannelNode>(uri =>
            {
                var transport = _transports.FirstOrDefault(x => x.Protocol == uri.Scheme);
                if (transport == null)
                {
                    throw new UnknownChannelException(uri);
                }

                var node = new ChannelNode { Uri = uri, Key = uri.ToString() };
                node.Channel = transport.BuildDestinationChannel(node.Uri);

                return node;
            });
        }

        public void ClearAll()
        {
            _lock.Write(() =>
            {
                _routes.Clear();
                _subscriptions.Clear();

                _volatileNodes.Each(x => x.Dispose());
                _volatileNodes.ClearAll();
            });
        }

        public IEnumerable<ChannelNode> FindDestinationChannels(Envelope envelope)
        {
            if (envelope.Destination != null)
            {
                var uri = envelope.Destination;
                var destination = findDestination(uri);

                return new ChannelNode[] { destination };
            }

            var inputType = envelope.Message.GetType();
            return _lock.MaybeWrite(() => _routes[inputType],
                () => !_routes.ContainsKey(inputType),
                () =>
                {
                    var nodes = FindSubscribingChannelsFor(inputType);
                    _routes.Add(inputType, new List<ChannelNode>(nodes));
                });
        }

        /// <summary>
        /// Called internally
        /// </summary>
        /// <param name="subscriptions"></param>
        public void LoadSubscriptions(IEnumerable<Subscription> subscriptions)
        {
            _lock.Write(() =>
            {
                RemoveCachedRoutesForChangedSubscriptions(subscriptions);

                _subscriptions.Clear();
                _subscriptions.AddRange(subscriptions);
            });
        }

        private void RemoveCachedRoutesForChangedSubscriptions(IEnumerable<Subscription> subscriptions)
        {
            _routes.Keys.ToList().Each(type =>
            {
                // This list of subscriptions is generally pretty small (less than 100),
                // so multiple enumerations over the collection shouldn't be a big deal.
                var existingSubscriptions = _subscriptions.Where(x => x.Matches(type));
                var newSubscriptions = subscriptions.Where(x => x.Matches(type));
                var differences = new HashSet<Subscription>(existingSubscriptions);
                differences.SymmetricExceptWith(newSubscriptions);
                if (differences.Any())
                {
                    _routes.Remove(type);
                }
            });
        }

        public IEnumerable<ChannelNode> FindSubscribingChannelsFor(Type inputType)
        {
            var staticNodes = _graph.Where(x => x.Publishes(inputType));

            var subscriptions = _subscriptions.Where(x => x.Matches(inputType));
            var dynamicNodes = subscriptions.Select(x => findDestination(x.Receiver));

            return staticNodes.Union(dynamicNodes).Distinct();


        }


        private ChannelNode findDestination(Uri uri)
        {
            return _graph.FirstOrDefault(x => x.Uri == uri) ?? _volatileNodes[uri];
        }

        public void Dispose()
        {
            _graph.Dispose();
        }


        public Uri ReplyUriFor(ChannelNode destination)
        {
            return _graph.ReplyChannelFor(destination.Protocol());
        }


        public IEnumerable<Subscription> ActiveSubscriptions
        {
            get
            {
                return _lock.Read(() => {
                    return _subscriptions.ToArray();
                });
            }
        }

        public string NodeName
        {
            get { return _graph.Name; }
        }

        // For testing
        public IDictionary<Type, IList<ChannelNode>> CachedRoutes
        {
            get { return _routes; }
        }
    }
}