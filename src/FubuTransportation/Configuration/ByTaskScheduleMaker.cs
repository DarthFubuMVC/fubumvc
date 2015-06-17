using System;
using System.Linq.Expressions;
using FubuTransportation.Scheduling;

namespace FubuTransportation.Configuration
{
    public class ByTaskScheduleMaker<T> : SchedulerMaker<T>
    {
        public ByTaskScheduleMaker(Expression<Func<T, int>> expression) : base(expression)
        {
        }

        protected override IScheduler buildScheduler(int threadCount)
        {
            return new TaskScheduler(threadCount);
        }
    }
}