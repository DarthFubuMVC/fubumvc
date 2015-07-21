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

        public Task<TaskHealthResponse> Handle(TaskHealthRequest request)
        {
            Debug.WriteLine("Received message {0} from {1}", request, _envelope.ReplyUri);

            return _controller.CheckStatusOfOwnedTasks().ContinueWith(t => {
                if (t.IsFaulted)
                {
                    return TaskHealthResponse.ErrorFor(request.Subjects);
                }

                var response = t.Result;
                response.AddMissingSubjects(request.Subjects);

                Debug.WriteLine("Responding with {0} on node {1} from health request from {2}", response, _graph.NodeId, _envelope.ReplyUri);

                return response;
            });
        }

        public Task<TaskDeactivationResponse> Handle(TaskDeactivation deactivation)
        {
            return
                _controller.Deactivate(deactivation.Subject)
                    .ContinueWith(
                        t => new TaskDeactivationResponse
                        {
                            Subject = deactivation.Subject, 
                            Success = t.Result
                        });
        }

        public Task<TakeOwnershipResponse> Handle(TakeOwnershipRequest request)
        {
            return _controller.TakeOwnership(request.Subject).ContinueWith(t => new TakeOwnershipResponse
            {
                NodeId = _graph.NodeId,
                Status = t.Result,
                Subject = request.Subject
            }).ContinueWith(t => {
                _logger.InfoMessage(() => {
                    var @event = new TakeOwnershipRequestReceived(request.Subject, _envelope.ReplyUri);
                    if (t.Result != null) @event.Status = t.Result.Status;

                    return @event;
                });

                return t.Result;
            });
        }
    }

    public class TaskDeactivationResponse
    {
        public Uri Subject { get; set; }
        public bool Success { get; set; }
    }
}