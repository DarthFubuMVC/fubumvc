using System;
using System.Linq.Expressions;
using FubuMVC.Core.ServiceBus.Configuration;

namespace FubuMVC.Core.ServiceBus.Polling
{
    public class PollingJobExpression
    {
        private readonly FubuTransportRegistry _parent;

        public PollingJobExpression(FubuTransportRegistry parent)
        {
            _parent = parent;
        }

        public IntervalExpression<TJob> RunJob<TJob>() where TJob : IJob
        {
            return new IntervalExpression<TJob>(this);
        } 

        public class IntervalExpression<TJob> where TJob : IJob
        {
            private readonly PollingJobExpression _parent;

            public IntervalExpression(PollingJobExpression parent)
            {
                _parent = parent;
            }

            public ScheduledExecutionExpression ScheduledAtInterval<TSettings>(
                Expression<Func<TSettings, double>> intervalInMillisecondsProperty)
            {
                _parent._parent.AlterSettings<PollingJobSettings>(x => {
                    var job = x.JobFor<TJob>();
                    job.SettingType = typeof (TSettings);
                    job.IntervalSource = intervalInMillisecondsProperty;
                });

                return new ScheduledExecutionExpression(_parent._parent);
            }

            public class ScheduledExecutionExpression
            {
                private readonly FubuTransportRegistry _registry;

                public ScheduledExecutionExpression(FubuTransportRegistry registry)
                {
                    _registry = registry;
                }

                private ScheduledExecution schedule
                {
                    set
                    {
                        _registry.AlterSettings<PollingJobSettings>(x => x.JobFor<TJob>().ScheduledExecution = value);
                    }
                }

                public void RunImmediately()
                {
                    schedule = ScheduledExecution.RunImmediately;
                }

                public void Disabled()
                {
                    schedule = ScheduledExecution.Disabled;
                }

                public void WaitForFirstInterval()
                {
                    schedule = ScheduledExecution.WaitUntilInterval;
                }
            }
        }
    }
}