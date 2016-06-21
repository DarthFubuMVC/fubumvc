using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FubuMVC.Core.ServiceBus.Monitoring
{
    public interface IPersistentTaskController
    {
        Task<HealthStatus> CheckStatus(Uri subject);
        Task<bool> Deactivate(Uri subject);
        Task EnsureTasksHaveOwnership();
        Task<OwnershipStatus> TakeOwnership(Uri subject);
        Task<TaskHealthResponse> CheckStatusOfOwnedTasks();
        IEnumerable<Uri> ActiveTasks();
        IEnumerable<Uri> PermanentTasks();
    }
}