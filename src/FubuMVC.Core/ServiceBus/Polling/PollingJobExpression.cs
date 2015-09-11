using System;
using System.Linq.Expressions;

namespace FubuMVC.Core.ServiceBus.Polling
{
    public class PollingJobExpression
    {
        private readonly FubuRegistry _parent;

        public PollingJobExpression(FubuRegistry parent)
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
                _parent._parent.AlterSettings<PollingJobSettings>(x => x.AddJob<TJob, TSettings>(intervalInMillisecondsProperty));

                return new ScheduledExecutionExpression(_parent._parent);
            }

            public class ScheduledExecutionExpression
            {
                private readonly FubuRegistry _registry;

                public ScheduledExecutionExpression(FubuRegistry registry)
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