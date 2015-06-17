using FubuTransportation.Polling;
using FubuTransportation.ScheduledJobs.Execution;

namespace FubuTransportation.ScheduledJobs.Messages
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