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

            if (!_node.Addresses.Any())
            {
                throw new ArgumentOutOfRangeException("node", "The TransportNode must have at least one reply Uri");
            }
        }

        public Task<OwnershipStatus> TakeOwnership(Uri subject)
        {
            _logger.InfoMessage(() => new TryingToAssignOwnership(subject, NodeId));

            return _serviceBus.Request<TakeOwnershipResponse>(new TakeOwnershipRequest(subject),
                new RequestOptions {Destination = ControlChannel, Timeout = _settings.TakeOwnershipMessageTimeout})
                .ContinueWith(t => {
                    if (t.IsFaulted)
                    {
                        _logger.Error(subject, "Unable to send the TakeOwnership message to node " + _node.NodeName, t.Exception);
                        return OwnershipStatus.Exception;
                    }

                    if (!t.IsCompleted)
                    {
                        Debug.WriteLine("TakeOwnership message for task {0} to node {1} timed out.", subject, _node.Id);
                        return OwnershipStatus.TimedOut;
                    }


                    Debug.WriteLine("TakeOwnership message for task {0} to node {1} returned status {2}", subject, _node.Id, t.Result.Status);
                    return t.Result.Status;
                });
        }

        public Task<TaskHealthResponse> CheckStatusOfOwnedTasks()
        {
            var subjects = CurrentlyOwnedSubjects().ToArray();

            if (!subjects.Any())
            {
                return Task.FromResult(TaskHealthResponse.Empty());
            }

            var request = new TaskHealthRequest
            {
                Subjects = subjects
            };

            return _serviceBus.Request<TaskHealthResponse>(request, new RequestOptions
            {
                Destination = ControlChannel,
                Timeout = _settings.HealthCheckMessageTimeout
            }).ContinueWith(t => {
                if (t.IsFaulted)
                {
                    _logger.Error(NodeId, "Could not retrieve persistent status checks", t.Exception);

                    t.Exception.Handle(e => true);

                    return TaskHealthResponse.ErrorFor(subjects);
                }

                if (t.IsCompleted)
                {
                    var response = t.Result;
                    response.AddMissingSubjects(subjects);

                    return response;
                }

                _logger.Info(() => "Persistent task health status timedout for node " + NodeId);
                return TaskHealthResponse.ErrorFor(subjects);
            });
        }

        public void RemoveOwnershipFromNode(IEnumerable<Uri> subjects)
        {
            _subscriptions.RemoveOwnershipFromNode(NodeId, subjects);
        }

        public IEnumerable<Uri> CurrentlyOwnedSubjects()
        {
            return _node.OwnedTasks;
        }


        public string NodeId
        {
            get
            {
                return _node.Id;
            }
        }

        public string MachineName
        {
            get
            {
                return _node.MachineName;
            }
        }

        public Uri ControlChannel
        {
            get
            {
                return _node.Addresses.FirstOrDefault();
            }
        }

        public Task<bool> Deactivate(Uri subject)
        {
            _logger.Info(() => "Requesting a deactivation of task {0} at node {1}".ToFormat(subject, NodeId));

            return _serviceBus.Request<TaskDeactivationResponse>(new TaskDeactivation(subject), new RequestOptions
            {
                Destination = ControlChannel,
                Timeout = _settings.DeactivationMessageTimeout
            }).ContinueWith(t => {
                if (t.IsFaulted)
                {
                    t.Exception.Handle(e => true);

                    _logger.Error(subject, "Failed while trying to deactivate a remote task", t.Exception);

                    _subscriptions.RemoveOwnershipFromNode(NodeId, subject);

                    return false;
                }
                else
                {
                    _subscriptions.RemoveOwnershipFromNode(NodeId, subject);

                    return true;
                }


            });
        }
    }
}