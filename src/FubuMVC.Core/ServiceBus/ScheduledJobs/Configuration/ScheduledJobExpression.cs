using System;
using System.Linq.Expressions;
using FubuCore.Reflection;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.Polling;
using FubuMVC.Core.ServiceBus.ScheduledJobs.Execution;

namespace FubuMVC.Core.ServiceBus.ScheduledJobs.Configuration
{
    public class ScheduledJobExpression<T>
    {
        private readonly FubuTransportRegistry _parent;
        private readonly ScheduledJobHandlerSource _scheduledJobs;

        public ScheduledJobExpression(FubuTransportRegistry parent, ScheduledJobHandlerSource scheduledJobs)
        {
            _parent = parent;
            _scheduledJobs = scheduledJobs;
        }

        public ScheduleExpression<TJob> RunJob<TJob>() where TJob : IJob
        {
            return new ScheduleExpression<TJob>(this);
        }


        public ScheduledJobExpression<T> DefaultJobChannel(Expression<Func<T, object>> channel)
        {
            _parent.AlterSettings<ScheduledJobGraph>(x => x.DefaultChannel = channel.ToAccessor());
            return this;
        } 

        public class ScheduleExpression<TJob> where TJob : IJob
        {
            private readonly ScheduledJobExpression<T> _parent;

            public ScheduleExpression(ScheduledJobExpression<T> parent)
            {
                _parent = parent;
            }



            public ChannelExpression ScheduledBy<TScheduler>() where TScheduler : IScheduleRule, new()
            {
                return ScheduledBy(new TScheduler());
            }

            public ChannelExpression ScheduledBy(IScheduleRule rule)
            {
                var job = new ScheduledJob<TJob>(rule);

                _parent._scheduledJobs.JobTypes.Add(typeof(TJob));
                _parent._parent.AlterSettings<ScheduledJobGraph>(x => x.Jobs.Add(job));

                return new ChannelExpression(job);
            }

            public class ChannelExpression 
            {
                private readonly ScheduledJob<TJob> _job;

                public ChannelExpression(ScheduledJob<TJob> job)
                {
                    _job = job;
                }

                public ChannelExpression Channel(Expression<Func<T, object>> channel)
                {
                    _job.Channel = channel.ToAccessor();
                    return this;
                }

                public ChannelExpression Timeout(TimeSpan timeout)
                {
                    _job.Timeout = timeout;
                    return this;
                }
            }
        }

        /// <summary>
        /// Disable the automatic startup of scheduled jobs. Valuable for testing
        /// </summary>
        /// <param name="shouldActivate"></param>
        /// <returns></returns>
        public ScheduledJobExpression<T> ActivatedOnStartup(bool shouldActivate)
        {
            _parent.HealthMonitoring.ScheduledExecution(shouldActivate ? ScheduledExecution.RunImmediately : ScheduledExecution.Disabled);
            return this;
        }
    }
}