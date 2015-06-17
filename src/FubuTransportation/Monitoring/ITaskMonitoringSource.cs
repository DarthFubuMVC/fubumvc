using System;
using System.Collections.Generic;

namespace FubuTransportation.Monitoring
{
    public interface ITaskMonitoringSource
    {
        IEnumerable<ITransportPeer> BuildPeers();

        IEnumerable<Uri> LocallyOwnedTasksAccordingToPersistence();

        IPersistentTaskAgent BuildAgentFor(IPersistentTask task);

        void RemoveOwnershipFromThisNode(IEnumerable<Uri> subjects);
    }
}