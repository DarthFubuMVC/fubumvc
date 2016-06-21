using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FubuMVC.Core.ServiceBus.Monitoring
{
    public interface IPersistentTasks
    {
        IPersistentTask FindTask(Uri subject);
        IPersistentTaskAgent FindAgent(Uri subject);
        string NodeId { get; }

        Task Reassign(Uri subject, IList<ITransportPeer> availablePeers, IList<ITransportPeer> deactivations);
    }
}