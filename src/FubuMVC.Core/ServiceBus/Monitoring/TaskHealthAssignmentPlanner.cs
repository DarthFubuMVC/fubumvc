using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using FubuCore.Util;

namespace FubuMVC.Core.ServiceBus.Monitoring
{
    public class TaskHealthAssignmentPlanner
    {
        private readonly IList<ITransportPeer> _availablePeers = new List<ITransportPeer>();

        private readonly Cache<Uri, TaskAssignmentStatus> _status
            = new Cache<Uri, TaskAssignmentStatus>(x => new TaskAssignmentStatus(x));

        private readonly Cache<ITransportPeer, List<Uri>> _removals = new Cache<ITransportPeer, List<Uri>>(_ => new List<Uri>());

        public TaskHealthAssignmentPlanner(IEnumerable<Uri> permanentTasks)
        {
            foreach (var task in permanentTasks)
            {
                _status.FillDefault(task);
            }

        }

        public void Add(ITransportPeer peer, TaskHealthResponse response)
        {
            var subjects = response.AllSubjects().ToArray();

            if (response.ResponseFailed)
            {
                Debug.WriteLine("Failed to detect available peer: " + peer.NodeId);
                _removals[peer].AddRange(subjects);
            }
            else
            {
                Debug.WriteLine("Detected an available peer: " + peer.NodeId);
                _availablePeers.Add(peer);

                response.Tasks.Each(x => _status[x.Subject].ReadPeerStatus(peer, x.Status));
            }
        }

        public IEnumerable<Task> ToCorrectionTasks(IPersistentTasks tasks)
        {
            if (_removals.Any())
            {
                yield return RemoveUnavailableOwnership();
            }

            foreach (var taskStatus in _status)
            {
                foreach (var task in taskStatus.ToTasks(tasks, _availablePeers))
                {
                    yield return task;
                }
            }
        }

        public Task RemoveUnavailableOwnership()
        {
            return
                Task.Factory.StartNew(
                    () => {
                        _removals.Each((node, subjects) => node.RemoveOwnershipFromNode(subjects));
                    });
        }
    }
}