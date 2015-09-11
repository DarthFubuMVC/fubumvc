using FubuMVC.Core.ServiceBus.Monitoring;
using FubuMVC.Core.ServiceBus.Polling;

namespace FubuMVC.Core.ServiceBus.Configuration
{
    public class HealthMonitoringExpression
    {
        private readonly FubuRegistry _parent;

        public HealthMonitoringExpression(FubuRegistry parent)
        {
            _parent = parent;
        }

        public HealthMonitoringExpression ScheduledExecution(ScheduledExecution scheduledExecution)
        {
            _parent.AlterSettings<PollingJobSettings>(x =>
            {
                x.AddJob<HealthMonitorPollingJob, HealthMonitoringSettings>(settings => settings.Interval)
                    .ScheduledExecution = scheduledExecution;
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