using System;
using System.Collections.Generic;

namespace FubuMVC.Core.ServiceBus.Monitoring
{
    public interface ITaskMonitoringSource
    {
        IEnumerable<ITransportPeer> BuildPeers();

        IEnumerable<Uri> LocallyOwnedTasksAccordingToPersistence();

        IPersistentTaskAgent BuildAgentFor(IPersistentTask task);

        void RemoveOwnershipFromThisNode(IEnumerable<Uri> subjects);
    }
}