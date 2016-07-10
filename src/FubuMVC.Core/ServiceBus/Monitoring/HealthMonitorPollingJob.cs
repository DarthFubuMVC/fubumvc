using System.Threading;
using FubuCore;
using FubuMVC.Core.ServiceBus.Polling;

namespace FubuMVC.Core.ServiceBus.Monitoring
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
            _controller.EnsureTasksHaveOwnership().Wait(2.Minutes());
        }
    }
}