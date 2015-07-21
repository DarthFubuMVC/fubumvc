using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FubuMVC.Core.ServiceBus.Monitoring
{
    public interface ITransportPeer
    {
        Task<OwnershipStatus> TakeOwnership(Uri subject);

        Task<TaskHealthResponse> CheckStatusOfOwnedTasks();

        void RemoveOwnershipFromNode(IEnumerable<Uri> subjects);

        IEnumerable<Uri> CurrentlyOwnedSubjects();

        string NodeId { get; }
        string MachineName { get; }
        Uri ControlChannel { get; }
        Task<bool> Deactivate(Uri subject);
    }
}