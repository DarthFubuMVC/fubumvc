using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FubuCore.Util;

namespace FubuMVC.Core.ServiceBus.Monitoring
{
    public class TaskHealthAssignmentPlanner
    {
        private readonly IEnumerable<Uri> _permanentTasks;
        private readonly IList<ITransportPeer> _availablePeers = new List<ITransportPeer>();
        private readonly IList<ITransportPeer> _unavailablePeers = new List<ITransportPeer>();

        private readonly Cache<Uri, TaskAssignmentStatus> _status
            = new Cache<Uri, TaskAssignmentStatus>(x => new TaskAssignmentStatus(x));

        private readonly Cache<ITransportPeer, List<Uri>> _removals = new Cache<ITransportPeer, List<Uri>>(_ => new List<Uri>());

        public TaskHealthAssignmentPlanner(IEnumerable<Uri> permanentTasks)
        {
            _permanentTasks = permanentTasks;
            _permanentTasks.Each(x => _status.FillDefault(x));
        }

        public void Add(ITransportPeer peer, TaskHealthResponse response)
        {

            var subjects = response.AllSubjects().ToArray();

            if (response.ResponseFailed)
            {
                _unavailablePeers.Add(peer);


                _removals[peer].AddRange(subjects);
            }
            else
            {
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

        public class TaskAssignmentStatus
        {
            public Uri Subject { get; private set; }

            public TaskAssignmentStatus(Uri subject)
            {
                Subject = subject;
            }

            public IList<ITransportPeer> ValidOwners = new List<ITransportPeer>();
            public IList<ITransportPeer> InvalidOwners = new List<ITransportPeer>();

            public IEnumerable<Task> ToTasks(IPersistentTasks tasks,
                IEnumerable<ITransportPeer> availablePeers)
            {
                switch (ValidOwners.Count)
                {
                    case 0:
                        yield return tasks.Reassign(Subject, availablePeers, InvalidOwners);
                        break;
                    case 1:
                        foreach (var transportPeer in InvalidOwners)
                        {
                            yield return transportPeer.Deactivate(Subject);
                        }
                        break;
                    default:
                        yield return tasks.Reassign(Subject, availablePeers, InvalidOwners.Union(ValidOwners));
                        break;
                }
            }


            public void ReadPeerStatus(ITransportPeer peer, HealthStatus status)
            {
                if (status == HealthStatus.Active)
                {
                    ValidOwners.Add(peer);
                }
                else
                {
                    InvalidOwners.Add(peer);
                }
            }
        }
    }
}