using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore.Logging;
using FubuMVC.Core.ServiceBus.Subscriptions;

namespace FubuMVC.Core.ServiceBus.Monitoring
{
    public class TaskMonitoringSource : ITaskMonitoringSource
    {
        private readonly HealthMonitoringSettings _settings;
        private readonly ILogger _logger;
        private readonly IServiceBus _serviceBus;
        private readonly ISubscriptionRepository _repository;

        public TaskMonitoringSource(HealthMonitoringSettings settings, ILogger logger, IServiceBus serviceBus, ISubscriptionRepository repository)
        {
            _settings = settings;
            _logger = logger;
            _serviceBus = serviceBus;
            _repository = repository;
        }

        public bool HasAnyPeers()
        {
            return _repository.FindPeers().Any();
        }

        private TransportPeer toPeer(TransportNode node)
        {
            return new TransportPeer(_settings, node, _repository, _serviceBus, _logger);
        }

        public IEnumerable<ITransportPeer> BuildPeers()
        {
            return _repository.FindPeers().Select(toPeer).ToArray();
        }

        public IEnumerable<Uri> LocallyOwnedTasksAccordingToPersistence()
        {
            return _repository.FindLocal().OwnedTasks;
        }

        public IPersistentTaskAgent BuildAgentFor(IPersistentTask task)
        {
            return new PersistentTaskAgent(task, _settings, _logger, _repository);
        }

        public void RemoveOwnershipFromThisNode(IEnumerable<Uri> subjects)
        {
            _repository.RemoveOwnershipFromThisNode(subjects);
        }
    }
}