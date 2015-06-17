using FubuTransportation.Monitoring;
using FubuTransportation.Polling;

namespace FubuTransportation.Configuration
{
    public class HealthMonitoringExpression
    {
        private readonly FubuTransportRegistry _parent;

        public HealthMonitoringExpression(FubuTransportRegistry parent)
        {
            _parent = parent;
        }

        public HealthMonitoringExpression ScheduledExecution(ScheduledExecution scheduledExecution)
        {
            _parent.AlterSettings<PollingJobSettings>(x => {
                x.JobFor<HealthMonitorPollingJob>().ScheduledExecution = scheduledExecution;
            });
            return this;
        }

        /// <summary>
        /// Seeds the random interval for health monitoring to a range of seconds
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public HealthMonitoringExpression IntervalSeed(int seconds)
        {
            _parent.AlterSettings<HealthMonitoringSettings>(x => x.Seed = seconds);
            return this;
        }
    }
}