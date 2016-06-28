using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FubuMVC.Core.ServiceBus.Monitoring
{
    public class TaskAssignmentStatus
    {
        public Uri Subject { get; private set; }

        public TaskAssignmentStatus(Uri subject)
        {
            Subject = subject;
        }

        public IList<ITransportPeer> ValidOwners = new List<ITransportPeer>();
        public IList<ITransportPeer> InvalidOwners = new List<ITransportPeer>();

        public IEnumerable<Task> ToTasks(IPersistentTasks tasks, IList<ITransportPeer> availablePeers)
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
                    yield return tasks.Reassign(Subject, availablePeers, InvalidOwners.Union(ValidOwners).ToList());
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