using System;
using System.Diagnostics;
using System.Threading.Tasks;
using FubuCore.Logging;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.Runtime;

namespace FubuMVC.Core.ServiceBus.Monitoring
{
    // Mostly tested through PersistentTaskController and/or integration tests
    public class MonitoringControlHandler
    {
        private readonly ILogger _logger;
        private readonly Envelope _envelope;
        private readonly ChannelGraph _graph;
        private readonly IPersistentTaskController _controller;

        public MonitoringControlHandler(ILogger logger, Envelope envelope, ChannelGraph graph, IPersistentTaskController controller)
        {
            _logger = logger;
            _envelope = envelope;
            _graph = graph;
            _controller = controller;
        }

        public async Task<TaskHealthResponse> Handle(TaskHealthRequest request)
        {
            Debug.WriteLine($"Received message {request} from {_envelope.ReplyUri}");

            try
            {
                var response = await _controller.CheckStatusOfOwnedTasks().ConfigureAwait(false);

                response.AddMissingSubjects(request.Subjects);

                Debug.WriteLine($"Responding from node {_graph.NodeId} with {response} from health request from {_envelope.ReplyUri}");

                return response;
            }
            catch (Exception e)
            {
                return TaskHealthResponse.ErrorFor(request.Subjects);
            }
        }

        public async Task<TaskDeactivationResponse> Handle(TaskDeactivation deactivation)
        {
            var status = await _controller.Deactivate(deactivation.Subject).ConfigureAwait(false);

            return new TaskDeactivationResponse {Subject = deactivation.Subject, Success = status};
        }

        public async Task<TakeOwnershipResponse> Handle(TakeOwnershipRequest request)
        {
            var status = await _controller.TakeOwnership(request.Subject).ConfigureAwait(false);

            var response = new TakeOwnershipResponse
            {
                NodeId = _graph.NodeId,
                Status = status,
                Subject = request.Subject
            };

            _logger.InfoMessage(() => new TakeOwnershipRequestReceived(request.Subject, _envelope.ReplyUri)
            {
                Status = response.Status
            });

            return response;
        }
    }
}