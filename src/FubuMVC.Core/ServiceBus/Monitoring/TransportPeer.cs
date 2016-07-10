using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using FubuCore;
using FubuCore.Logging;
using FubuMVC.Core.ServiceBus.Subscriptions;

namespace FubuMVC.Core.ServiceBus.Monitoring
{
    public class TransportPeer : ITransportPeer
    {
        private readonly HealthMonitoringSettings _settings;
        private readonly TransportNode _node;
        private readonly ISubscriptionRepository _subscriptions;
        private readonly IServiceBus _serviceBus;
        private readonly ILogger _logger;

        public TransportPeer(HealthMonitoringSettings settings, TransportNode node, ISubscriptionRepository subscriptions, IServiceBus serviceBus, ILogger logger)
        {
            _settings = settings;
            _node = node;
            _subscriptions = subscriptions;
            _serviceBus = serviceBus;
            _logger = logger;

            if (_node.ControlChannel == null && !_node.Addresses.Any())
            {
                throw new ArgumentOutOfRangeException(nameof(node), "The TransportNode must have at least one reply Uri");
            }
        }

        public async Task<OwnershipStatus> TakeOwnership(Uri subject)
        {
            _logger.InfoMessage(() => new TryingToAssignOwnership(subject, NodeId));

            try
            {
                var response = await _serviceBus.Request<TakeOwnershipResponse>(new TakeOwnershipRequest(subject), new RequestOptions
                {
                    Destination = ControlChannel,
                    Timeout = _settings.TakeOwnershipMessageTimeout
                }).ConfigureAwait(false);

                if (response == null)
                {
                    Debug.WriteLine($"TakeOwnership message for task {subject} to node {_node.Id} timed out.");
                    return OwnershipStatus.TimedOut;
                }

                Debug.WriteLine($"TakeOwnership message for task {subject} to node {_node.Id} returned status {response.Status}");

                return response.Status;
            }
            catch (Exception e)
            {
                _logger.Error(subject, "Unable to send the TakeOwnership message to node " + _node.NodeName, e);
                return OwnershipStatus.Exception;
            }
        }

        public async Task<TaskHealthResponse> CheckStatusOfOwnedTasks()
        {
            var subjects = CurrentlyOwnedSubjects().ToArray();

            if (!subjects.Any())
            {
                return TaskHealthResponse.Empty();
            }

            var request = new TaskHealthRequest
            {
                Subjects = subjects
            };


            try
            {
                var response = await _serviceBus.Request<TaskHealthResponse>(request, new RequestOptions
                {
                    Destination = ControlChannel,
                    Timeout = _settings.HealthCheckMessageTimeout
                }).ConfigureAwait(false);

                if (response == null)
                {
                    _logger.Info(() => "Persistent task health status timedout for node " + NodeId);
                    return TaskHealthResponse.ErrorFor(subjects);
                }

                response.AddMissingSubjects(subjects);

                return response;
            }
            catch (Exception e)
            {
                _logger.Error(NodeId, "Could not retrieve persistent status checks", e);

                return TaskHealthResponse.ErrorFor(subjects);
            }
        }

        public void RemoveOwnershipFromNode(IEnumerable<Uri> subjects)
        {
            _subscriptions.RemoveOwnershipFromNode(NodeId, subjects);
        }

        public IEnumerable<Uri> CurrentlyOwnedSubjects()
        {
            return _node.OwnedTasks;
        }


        public string NodeId => _node.Id;

        public string MachineName => _node.MachineName;

        public Uri ControlChannel => _node.ControlChannel;

        public async Task<bool> Deactivate(Uri subject)
        {
            _logger.Info(() => "Requesting a deactivation of task {0} at node {1}".ToFormat(subject, NodeId));

            try
            {
                var response = await _serviceBus.Request<TaskDeactivationResponse>(new TaskDeactivation(subject), new RequestOptions
                {
                    Destination = ControlChannel,
                    Timeout = _settings.DeactivationMessageTimeout
                }).ConfigureAwait(false);

                _subscriptions.RemoveOwnershipFromNode(NodeId, subject);

                return true;

            }
            catch (Exception e)
            {
                _logger.Error(subject, "Failed while trying to deactivate a remote task", e);

                _subscriptions.RemoveOwnershipFromNode(NodeId, subject);

                return false;
            }
        }

        public override string ToString()
        {
            return $"Remote TransportPeer: {NodeId}";
        }
    }
}