using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.ServiceBus.Runtime;

namespace FubuMVC.Core.ServiceBus.Subscriptions
{
    public class SubscriptionsHandler
    {
        private readonly ISubscriptionRepository _repository;
        private readonly ISubscriptionCache _cache;
        private readonly IEnvelopeSender _sender;

        public SubscriptionsHandler(ISubscriptionRepository repository, ISubscriptionCache cache, IEnvelopeSender sender)
        {
            _repository = repository;
            _cache = cache;
            _sender = sender;
        }

        public void Handle(SubscriptionsChanged message)
        {
            ReloadSubscriptions();
        }

        public virtual void ReloadSubscriptions()
        {
            var subscriptions = _repository.LoadSubscriptions(SubscriptionRole.Publishes);
            _cache.LoadSubscriptions(subscriptions);
        }

        public void Handle(SubscriptionRequested message)
        {
            _repository.PersistPublishing(message.Subscriptions);
            
            // Reload here!
            Handle(new SubscriptionsChanged());

            UpdatePeers();
        }

        public virtual void UpdatePeers()
        {
            var peers = _repository.FindPeers();
            peers.Each(SendSubscriptionChangedToPeer);
        }

        public virtual void SendSubscriptionChangedToPeer(TransportNode node)
        {
            var envelope = new Envelope
            {
                Message = new SubscriptionsChanged(),
                Destination = node.Addresses.FirstOrDefault()
            };

            _sender.Send(envelope);
        }

        public void Handle(SubscriptionsRemoved message)
        {
            _repository.RemoveSubscriptionsForReceiver(message.Receiver);
            ReloadSubscriptions();
            UpdatePeers();
        }
    }
}