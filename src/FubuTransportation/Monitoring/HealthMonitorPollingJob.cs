using System.Threading;
using FubuTransportation.Polling;

namespace FubuTransportation.Monitoring
{
    public class HealthMonitorPollingJob : IJob
    {
        private readonly IPersistentTaskController _controller;

        public HealthMonitorPollingJob(IPersistentTaskController controller)
        {
            _controller = controller;
        }

        public void Execute(CancellationToken cancellation)
        {
            _controller.EnsureTasksHaveOwnership();
        }
    }
}