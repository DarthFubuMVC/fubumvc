using System;
using System.Linq.Expressions;
using FubuCore.Reflection;
using FubuTransportation.Scheduling;

namespace FubuTransportation.Configuration
{
    public abstract class SchedulerMaker<T> : ISettingsAware
    {
        private readonly Expression<Func<T, int>> _expression;

        public SchedulerMaker(Expression<Func<T, int>> expression)
        {
            _expression = expression;
        }

        void ISettingsAware.ApplySettings(object settings, ChannelNode node)
        {
            int threadCount = (int) ReflectionHelper.GetAccessor(_expression).GetValue(settings);
            node.Scheduler = buildScheduler(threadCount);
        }

        protected abstract IScheduler buildScheduler(int threadCount);
    }
}