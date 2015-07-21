using FubuMVC.Core.ServiceBus.Polling;
using FubuMVC.Core.ServiceBus.ScheduledJobs.Execution;

namespace FubuMVC.Core.ServiceBus.ScheduledJobs.Messages
{

    public class SchedulingHandler<T> where T : IJob
    {
        private readonly IScheduledJobController _controller;

        public SchedulingHandler(IScheduledJobController controller)
        {
            _controller = controller;
        }

        public void Reschedule(RescheduleRequest<T> request)
        {
            _controller.Reschedule(request);
        }
    }
}