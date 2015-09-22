using System;
using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Diagnostics.Packaging;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.Runtime;

namespace FubuMVC.Core.ServiceBus.Subscriptions
{
    // Tested through Storyteller tests
    public class SubscriptionActivator : IActivator
    {
        private readonly ISubscriptionRepository _repository;
        private readonly IEnvelopeSender _sender;
        private readonly ISubscriptionCache _cache;
        private readonly IEnumerable<ISubscriptionRequirement> _requirements;
        private readonly ChannelGraph _graph;
        private readonly TransportSettings _settings;

        public SubscriptionActivator(ISubscriptionRepository repository, IEnvelopeSender sender, ISubscriptionCache cache, IEnumerable<ISubscriptionRequirement> requirements, ChannelGraph graph,
            TransportSettings settings)
        {
            _repository = repository;
            _sender = sender;
            _cache = cache;
            _requirements = requirements;
            _graph = graph;
            _settings = settings;
        }

        public void Activate(IActivationLog log, IPerfTimer timer)
        {
            if (!_graph.HasChannels) return;

            log.Trace("Determining subscriptions for node " + _cache.NodeName);

            // assuming that there are no automaticly persistent tasks
            // upon startup
            _repository.Persist(new TransportNode(_graph));

            var requirements = determineStaticRequirements(log);


            if (requirements.Any())
            {
                log.Trace("Found static subscription requirements:");
                requirements.Each(x => log.Trace(x.ToString()));
            }
            else
            {
                log.Trace("No static subscriptions found from registry");
            }

            _repository.PersistSubscriptions(requirements);

            var subscriptions = _repository.LoadSubscriptions(SubscriptionRole.Publishes);
            _cache.LoadSubscriptions(subscriptions);

            sendSubscriptions();
        }

        private Subscription[] determineStaticRequirements(IActivationLog log)
        {
            var requirements = _requirements.SelectMany(x => x.DetermineRequirements()).ToArray();
            traceLoadedRequirements(log, requirements);
            return requirements;
        }

        private void sendSubscriptions()
        {
            _repository.LoadSubscriptions(SubscriptionRole.Subscribes)
                .GroupBy(x => x.Source)
                .Each(group => sendSubscriptionsToSource(@group.Key, @group));
        }

        private static void traceLoadedRequirements(IActivationLog log, Subscription[] requirements)
        {
            log.Trace("Found subscription requirements:");
            requirements.Each(x => log.Trace(x.ToString()));
        }

        private void sendSubscriptionsToSource(Uri destination, IEnumerable<Subscription> subscriptions)
        {
            var envelope = new Envelope
            {
                Message = new SubscriptionRequested
                {
                    Subscriptions = subscriptions.ToArray()
                },
                Destination = destination
            };

            _sender.Send(envelope);
        }
    }
}