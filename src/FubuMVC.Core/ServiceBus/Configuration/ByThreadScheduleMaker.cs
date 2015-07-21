using System;
using System.Linq.Expressions;
using FubuMVC.Core.ServiceBus.Scheduling;

namespace FubuMVC.Core.ServiceBus.Configuration
{
    public class ByThreadScheduleMaker<T> : SchedulerMaker<T>
    {
        public ByThreadScheduleMaker(Expression<Func<T, int>> expression) : base(expression)
        {
        }

        protected override IScheduler buildScheduler(int threadCount)
        {
            return new ThreadScheduler(threadCount);
        }
    }
}